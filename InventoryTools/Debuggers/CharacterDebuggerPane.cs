using System.Collections.Generic;
using AllaganLib.Shared.Debuggers;
using AllaganLib.Shared.Interfaces;
using CriticalCommonLib;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using InventoryTools.Localization;

namespace InventoryTools.Debuggers;

public class CharacterDebuggerPane : DebugLogPane
{
    private readonly ICharacterMonitor _characterMonitor;
    private readonly IClientState _clientState;
    private readonly InventoryToolsConfiguration _configuration;

    public CharacterDebuggerPane(ICharacterMonitor characterMonitor, IClientState clientState, InventoryToolsConfiguration configuration)
    {
        _characterMonitor = characterMonitor;
        _clientState = clientState;
        _configuration = configuration;
    }
    public override string Name => "角色监控";

    public override void SubscribeToEvents()
    {
        _characterMonitor.OnActiveRetainerChanged += OnCharacterMonitorOnOnActiveRetainerChanged;
        RegisterSubscription(() => _characterMonitor.OnActiveRetainerChanged -= OnCharacterMonitorOnOnActiveRetainerChanged);

        _characterMonitor.OnActiveRetainerLoaded += OnCharacterMonitorOnOnActiveRetainerLoaded;
        RegisterSubscription(() => _characterMonitor.OnActiveRetainerLoaded -= OnCharacterMonitorOnOnActiveRetainerLoaded);

        _characterMonitor.OnActiveFreeCompanyChanged += OnCharacterMonitorOnOnActiveFreeCompanyChanged;
        RegisterSubscription(() => _characterMonitor.OnActiveFreeCompanyChanged -= OnCharacterMonitorOnOnActiveFreeCompanyChanged);

        _characterMonitor.OnActiveHouseChanged += OnCharacterMonitorOnOnActiveHouseChanged;
        RegisterSubscription(() => _characterMonitor.OnActiveHouseChanged -= OnCharacterMonitorOnOnActiveHouseChanged);

        _characterMonitor.OnCharacterUpdated += OnCharacterMonitorOnOnCharacterUpdated;
        RegisterSubscription(() => _characterMonitor.OnCharacterUpdated -= OnCharacterMonitorOnOnCharacterUpdated);

        _characterMonitor.OnCharacterRemoved += OnCharacterMonitorOnOnCharacterRemoved;
        RegisterSubscription(() => _characterMonitor.OnCharacterRemoved -= OnCharacterMonitorOnOnCharacterRemoved);

        _characterMonitor.OnCharacterJobChanged += OnCharacterJobChanged;
        RegisterSubscription(() => _characterMonitor.OnCharacterJobChanged -= OnCharacterJobChanged);

        _characterMonitor.OnCharacterLoggedIn += OnCharacterMonitorOnOnCharacterLoggedIn;
        RegisterSubscription(() => _characterMonitor.OnCharacterLoggedIn -= OnCharacterMonitorOnOnCharacterLoggedIn);

        _characterMonitor.OnCharacterLoggedOut += OnCharacterMonitorOnOnCharacterLoggedOut;
        RegisterSubscription(() => _characterMonitor.OnCharacterLoggedOut -= OnCharacterMonitorOnOnCharacterLoggedOut);
    }

    private void OnCharacterMonitorOnOnCharacterLoggedOut(ulong id)
    {
        AddLog($"Character logged out: {id}");
    }

    private void OnCharacterMonitorOnOnCharacterLoggedIn(ulong id)
    {
        AddLog($"Character logged in: {id}");
    }

    private void OnCharacterJobChanged()
    {
        AddLog($"Character job changed");
    }

    private void OnCharacterMonitorOnOnCharacterRemoved(ulong id)
    {
        AddLog($"Character removed: {id}");
    }

    private void OnCharacterMonitorOnOnCharacterUpdated(Character? c)
    {
        AddLog($"Character updated: {c}");
    }

    private void OnCharacterMonitorOnOnActiveHouseChanged(ulong houseId, sbyte wardId, sbyte plotId, byte divisionId, short roomId, bool hasHousePermission)
    {
        AddLog($"Active house changed: {houseId}, {wardId}, {plotId}, {divisionId}, {roomId}, {hasHousePermission}");
    }

    private void OnCharacterMonitorOnOnActiveFreeCompanyChanged(ulong c)
    {
        AddLog($"Active FC changed: {c}");
    }

    private void OnCharacterMonitorOnOnActiveRetainerLoaded(ulong c)
    {
        AddLog($"Active retainer loaded: {c}");
    }

    private void OnCharacterMonitorOnOnActiveRetainerChanged(ulong c)
    {
        AddLog($"Active retainer changed: {c}");
    }

    public override unsafe void DrawInfo()
    {
        if (ImGui.CollapsingHeader("会话/当前状态"))
        {
            ImGui.TextUnformatted(string.Format("已登录：{0}", _characterMonitor.IsLoggedIn));
            ImGui.TextUnformatted(string.Format("本地内容 ID：{0}", _characterMonitor.LocalContentId));
            ImGui.TextUnformatted(string.Format("内部角色 ID：{0}", _characterMonitor.InternalCharacterId));

            ImGui.Separator();
            ImGui.TextUnformatted("当前角色：");
            ImGui.TextUnformatted(_characterMonitor.ActiveCharacter != null
                ? $"{_characterMonitor.ActiveCharacter.Name} ({_characterMonitor.ActiveCharacterId})"
                : "<无>");

            ImGui.TextUnformatted("当前雇员：");
            ImGui.TextUnformatted(_characterMonitor.ActiveRetainer != null
                ? $"{_characterMonitor.ActiveRetainer.Name} ({_characterMonitor.ActiveRetainerId})"
                : "<无>");

            ImGui.TextUnformatted("当前部队：");
            ImGui.TextUnformatted(_characterMonitor.ActiveFreeCompany != null
                ? $"{_characterMonitor.ActiveFreeCompany.Name} ({_characterMonitor.ActiveFreeCompanyId})"
                : "<无>");
        }

        if (ImGui.CollapsingHeader("房屋"))
        {
            ImGui.TextUnformatted(string.Format("当前房屋 ID：{0}", _characterMonitor.ActiveHouseId));
            ImGui.TextUnformatted(string.Format("缓存分区 ID：{0}", _characterMonitor.InternalWardId));
            ImGui.TextUnformatted(string.Format("缓存土地 ID：{0}", _characterMonitor.InternalPlotId));
            ImGui.TextUnformatted(string.Format("缓存分队 ID：{0}", _characterMonitor.InternalDivisionId));
            ImGui.TextUnformatted(string.Format("缓存房间 ID：{0}", _characterMonitor.InternalRoomId));
            ImGui.TextUnformatted(string.Format("缓存房屋 ID：{0}", _characterMonitor.InternalHouseId));
            ImGui.TextUnformatted(string.Format("区域类型 ID：{0}", _characterMonitor.CorrectedTerritoryTypeId));

            var hm = HousingManager.Instance();
            if (hm != null)
            {
                if (hm->OutdoorTerritory != null)
                    ImGui.TextUnformatted($"Outdoor HouseId: {hm->OutdoorTerritory->HouseId.Id}");
                if (hm->IndoorTerritory != null)
                    ImGui.TextUnformatted($"Indoor HouseId: {hm->IndoorTerritory->HouseId.Id}");
                if (hm->CurrentTerritory != null)
                    ImGui.TextUnformatted($"Current Territory: {(ulong)hm->CurrentTerritory:X}");
            }

            ImGui.Separator();
            ImGui.TextUnformatted("拥有的房屋：");
            foreach (var id in _characterMonitor.GetOwnedHouseIds())
                ImGui.BulletText(id.ToString());

            ImGui.TextUnformatted("拥有房屋权限：" +
                (_characterMonitor.InternalHasHousePermission ||
                 _characterMonitor.GetOwnedHouseIds().Contains(_characterMonitor.InternalHouseId)
                    ? "Yes"
                    : "No"));
        }

        //
        // Worlds
        //
        if (ImGui.CollapsingHeader("服务器"))
        {
            foreach (var wid in _characterMonitor.GetWorldIds())
                ImGui.BulletText(string.Format("服务器 {0}", wid));
        }

        if (ImGui.CollapsingHeader("角色"))
        {
            foreach (var kv in _characterMonitor.Characters)
                ImGui.BulletText(string.Format("{0}: {1}", kv.Key, kv.Value.Name));
        }

        if (ImGui.CollapsingHeader("雇员"))
        {
            using (var table = ImRaii.Table("retainerTable", 6))
            {
                if (table)
                {
                    ImGui.TableSetupColumn("雇用顺序");
                    ImGui.TableSetupColumn("名称");
                    ImGui.TableSetupColumn("类型");
                    ImGui.TableSetupColumn("金币");
                    ImGui.TableSetupColumn("ID");
                    ImGui.TableSetupColumn("持有者 ID");
                    ImGui.TableHeadersRow();

                    foreach (var retainer in _characterMonitor.GetRetainerCharacters())
                    {
                        if (retainer.Value.Name == "Unhired")
                            continue;

                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted((retainer.Value.HireOrder + 1).ToString());

                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted(retainer.Value.CharacterType == CharacterType.Housing
                            ? retainer.Value.HousingName
                            : retainer.Value.Name);

                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted(retainer.Value.CharacterType.ToString());

                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted(retainer.Value.Gil.ToString());

                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted(retainer.Value.CharacterId.ToString());

                        ImGui.TableNextColumn();
                        ImGui.TextUnformatted(retainer.Value.OwnerId.ToString());
                    }
                }
            }
        }

        if (ImGui.CollapsingHeader("角色物品"))
        {
            foreach (var kv in _characterMonitor.Characters)
            {
                var label = kv.Value.CharacterType == CharacterType.Housing
                    ? kv.Value.HousingName
                    : kv.Value.Name;

                if (ImGui.TreeNode(string.Format("{0}##{1}", label, kv.Key)))
                {
                    Utils.PrintOutObject(kv.Value, 0, new List<string>());
                    ImGui.TreePop();
                }
            }
        }

        if (ImGui.CollapsingHeader("已获得物品"))
        {
            foreach (var characterPair in _configuration.AcquiredItems)
            {
                var character = _characterMonitor.GetCharacterById(characterPair.Key);
                ImGui.TextUnformatted(character?.FormattedName ?? "未知角色");
                ImGui.Text(string.Format("{0} 件已解锁物品", characterPair.Value.Count));
            }
        }
    }
}