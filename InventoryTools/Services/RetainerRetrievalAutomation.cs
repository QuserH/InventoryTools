using System;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Services;
using Dalamud.Plugin.Services;

namespace InventoryTools.Services;

/// <summary>
/// Framework-thread state machine that walks a retrieval plan the same way a player would:
/// bell -> retainer list -> retainer -> entrust/withdraw -> per-item retrieve -> quit -> next retainer.
/// It never advances until the expected UI transition is actually visible, and every state has a
/// timeout so a missed window stops the run instead of clicking blindly.
/// </summary>
public sealed class RetainerRetrievalAutomation : IDisposable
{
    private const long StateTimeoutMs = 20_000;
    private const long ActionThrottleMs = 500;

    private readonly IFramework _framework;
    private readonly IRetainerGameUi _ui;
    private readonly RetainerRetrievalPlanner _planner;
    private readonly ICharacterMonitor _characters;
    private readonly IPluginLog _log;

    private RetainerRetrievalPlan? _plan;
    private int _entryIndex;
    private uint _remainingForEntry;
    private uint _pendingQuantity;
    private uint _retainerQuantityBeforeTransfer;
    private long _stateStartedMs;
    private long _lastActionMs;

    public RetainerRetrievalAutomation(IFramework framework, IRetainerGameUi ui, RetainerRetrievalPlanner planner,
        ICharacterMonitor characters, IPluginLog log)
    {
        _framework = framework;
        _ui = ui;
        _planner = planner;
        _characters = characters;
        _log = log;
        _framework.Update += Update;
    }

    public RetainerRetrievalState State { get; private set; } = RetainerRetrievalState.Idle;

    public string Status { get; private set; } = "空闲";

    public bool IsRunning => State is not (RetainerRetrievalState.Idle or RetainerRetrievalState.Completed
        or RetainerRetrievalState.Failed or RetainerRetrievalState.Cancelled);

    public bool Start(CraftList craftList)
    {
        if (IsRunning)
            return false;
        if (!_characters.IsLoggedIn)
        {
            Status = "未登录角色";
            return false;
        }
        if (_ui.IsOccupiedAtBell)
        {
            Status = "请先关闭当前雇员界面，再从传唤铃旁开始取回";
            return false;
        }

        _plan = _planner.Build(craftList);
        if (_plan.IsEmpty)
        {
            Status = "当前角色的雇员中没有清单需要取回的材料";
            return false;
        }

        _entryIndex = 0;
        _remainingForEntry = _plan.Entries[0].Quantity;
        _log.Information($"Retainer retrieval started: {_plan.Entries.Count} entries, {_plan.TotalQuantity} items total");
        SetState(RetainerRetrievalState.FindingBell, "正在寻找附近的传唤铃");
        return true;
    }

    public void Cancel()
    {
        if (!IsRunning)
            return;
        _ui.TryCloseRetainerInventory();
        SetState(RetainerRetrievalState.Cancelled, "已取消雇员取回");
    }

    private void Update(IFramework framework)
    {
        if (!IsRunning || _plan == null)
            return;

        var now = Environment.TickCount64;
        if (now - _stateStartedMs > StateTimeoutMs)
        {
            _log.Warning($"Retainer retrieval timed out in state {State}");
            _ui.TryCloseRetainerInventory();
            SetState(RetainerRetrievalState.Failed, $"等待游戏界面超时（{StateDescription(State)}），已停止");
            return;
        }

        try
        {
            Advance(now);
        }
        catch (Exception exception)
        {
            _log.Error(exception, "Retainer retrieval automation stopped unexpectedly");
            _ui.TryCloseRetainerInventory();
            SetState(RetainerRetrievalState.Failed, "雇员取回发生错误，已停止");
        }
    }

    private void Advance(long now)
    {
        var entry = _plan!.Entries[_entryIndex];
        switch (State)
        {
            case RetainerRetrievalState.FindingBell:
                if (_ui.IsOccupiedAtBell)
                {
                    SetState(RetainerRetrievalState.WaitingForRetainerList, "正在等待雇员列表");
                }
                else if (CanAct(now) && _ui.TryInteractWithNearestBell())
                {
                    SetState(RetainerRetrievalState.WaitingForRetainerList, "正在打开雇员列表");
                }
                else if (CanAct(now))
                {
                    Status = "附近没有可交互的传唤铃，请站到传唤铃旁边";
                }
                break;

            case RetainerRetrievalState.WaitingForRetainerList:
                if (CanAct(now))
                    _ui.TryAdvanceBellTalk();
                if (_ui.IsRetainerListReady)
                    SetState(RetainerRetrievalState.SelectingRetainer, $"正在呼叫雇员 {RetainerName(entry.RetainerId)}");
                break;

            case RetainerRetrievalState.SelectingRetainer:
                if (CanAct(now) && _ui.TrySelectRetainer(entry.RetainerId))
                    SetState(RetainerRetrievalState.SelectingRetainerAction, "正在选择“道具的交易”");
                break;

            case RetainerRetrievalState.SelectingRetainerAction:
                if (CanAct(now) && _ui.TrySelectEntrustOrWithdraw())
                    SetState(RetainerRetrievalState.WaitingForInventory, "正在打开雇员背包");
                break;

            case RetainerRetrievalState.WaitingForInventory:
                if (_ui.IsRetainerInventoryReady)
                    SetState(RetainerRetrievalState.RetrievingItem, "正在取回清单材料");
                break;

            case RetainerRetrievalState.RetrievingItem:
                if (CanAct(now))
                {
                    _retainerQuantityBeforeTransfer = _ui.GetRetainerItemQuantity(entry.ItemId, entry.Flags);
                    if (_ui.TryRetrieve(entry.ItemId, entry.Flags, _remainingForEntry,
                            out var expectsQuantity, out var willRetrieve))
                    {
                        _pendingQuantity = Math.Min(_remainingForEntry, willRetrieve);
                        if (expectsQuantity)
                            SetState(RetainerRetrievalState.WaitingForQuantityInput, "正在输入取回数量");
                        else
                            SetState(RetainerRetrievalState.WaitingForInventoryChange, "正在等待物品转移");
                    }
                }
                break;

            case RetainerRetrievalState.WaitingForQuantityInput:
                if (CanAct(now) && _ui.TrySetQuantity(_pendingQuantity))
                    SetState(RetainerRetrievalState.WaitingForInventoryChange, "正在等待物品转移");
                break;

            case RetainerRetrievalState.WaitingForInventoryChange:
                if (!HasExpectedTransferCompleted(entry))
                    break;
                _remainingForEntry = _pendingQuantity >= _remainingForEntry ? 0 : _remainingForEntry - _pendingQuantity;
                if (_remainingForEntry > 0)
                {
                    SetState(RetainerRetrievalState.RetrievingItem, "正在继续取回同一材料");
                    break;
                }

                if (TryAdvanceEntry(out var sameRetainer))
                {
                    SetState(sameRetainer
                            ? RetainerRetrievalState.RetrievingItem
                            : RetainerRetrievalState.ClosingRetainerInventory,
                        sameRetainer ? "正在取回下一种材料" : "正在返回雇员菜单");
                }
                else
                {
                    SetState(RetainerRetrievalState.ClosingRetainerInventory, "取回完成，正在关闭雇员背包");
                }
                break;

            case RetainerRetrievalState.ClosingRetainerInventory:
                if (CanAct(now) && _ui.TryCloseRetainerInventory())
                    SetState(RetainerRetrievalState.SelectingQuit, "正在退出当前雇员");
                break;

            case RetainerRetrievalState.SelectingQuit:
                if (CanAct(now) && _ui.TrySelectQuit())
                {
                    if (_entryIndex < _plan.Entries.Count)
                        SetState(RetainerRetrievalState.WaitingForRetainerList, "正在切换下一个雇员");
                    else
                        SetState(RetainerRetrievalState.ClosingRetainerList, "正在关闭雇员列表");
                }
                break;

            case RetainerRetrievalState.ClosingRetainerList:
                if (CanAct(now) && _ui.TryCloseRetainerList())
                {
                    _log.Information("Retainer retrieval completed");
                    SetState(RetainerRetrievalState.Completed, "雇员材料取回完成");
                }
                break;
        }
    }

    /// <summary>
    /// Moves to the next plan entry. Returns false when the plan is exhausted; sameRetainer reports
    /// whether the next entry can be handled without leaving the currently open retainer.
    /// </summary>
    private bool TryAdvanceEntry(out bool sameRetainer)
    {
        var previous = _plan!.Entries[_entryIndex];
        _entryIndex++;
        if (_entryIndex >= _plan.Entries.Count)
        {
            sameRetainer = false;
            return false;
        }

        var next = _plan.Entries[_entryIndex];
        _remainingForEntry = next.Quantity;
        sameRetainer = next.RetainerId == previous.RetainerId;
        return true;
    }

    private bool CanAct(long now)
    {
        if (now - _lastActionMs < ActionThrottleMs)
            return false;
        _lastActionMs = now;
        return true;
    }

    private bool HasExpectedTransferCompleted(RetainerRetrievalEntry entry)
    {
        var current = _ui.GetRetainerItemQuantity(entry.ItemId, entry.Flags);
        return current <= _retainerQuantityBeforeTransfer - Math.Min(_retainerQuantityBeforeTransfer, _pendingQuantity);
    }

    private string RetainerName(ulong retainerId) => _characters.GetCharacterNameById(retainerId);

    private static string StateDescription(RetainerRetrievalState state)
    {
        return state switch
        {
            RetainerRetrievalState.FindingBell => "传唤铃",
            RetainerRetrievalState.WaitingForRetainerList => "雇员列表",
            RetainerRetrievalState.SelectingRetainer => "选择雇员",
            RetainerRetrievalState.SelectingRetainerAction => "雇员菜单",
            RetainerRetrievalState.WaitingForInventory => "雇员背包",
            RetainerRetrievalState.RetrievingItem => "取回道具",
            RetainerRetrievalState.WaitingForQuantityInput => "数量输入",
            RetainerRetrievalState.WaitingForInventoryChange => "物品转移",
            RetainerRetrievalState.ClosingRetainerInventory => "关闭背包",
            RetainerRetrievalState.SelectingQuit => "退出雇员",
            RetainerRetrievalState.ClosingRetainerList => "关闭列表",
            _ => state.ToString(),
        };
    }

    private void SetState(RetainerRetrievalState state, string status)
    {
        State = state;
        Status = status;
        _stateStartedMs = Environment.TickCount64;
    }

    public void Dispose()
    {
        _framework.Update -= Update;
    }
}
