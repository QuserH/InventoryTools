using System;
using System.Numerics;
using CriticalCommonLib.Services;
using Dalamud.Game.ClientState.Conditions;
using Dalamud.Game.ClientState.Objects.Enums;
using Dalamud.Memory;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.Control;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.Sheets;
using InventoryItem = FFXIVClientStructs.FFXIV.Client.Game.InventoryItem;
using InventoryType = FFXIVClientStructs.FFXIV.Client.Game.InventoryType;

namespace InventoryTools.Services;

/// <summary>
/// Framework-thread native actions used by the retainer retrieval state machine. Every action
/// verifies the exact addon it expects before firing a callback, so a stale state can never
/// click into an unrelated window.
/// </summary>
public interface IRetainerGameUi
{
    bool IsOccupiedAtBell { get; }
    bool IsRetainerListReady { get; }
    bool IsRetainerInventoryReady { get; }
    bool TryInteractWithNearestBell();
    bool TryAdvanceBellTalk();
    bool TrySelectRetainer(ulong retainerId);
    bool TrySelectEntrustOrWithdraw();
    bool TryRetrieve(uint itemId, FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags flags, uint quantity, out bool expectsQuantityInput, out uint willRetrieve);
    uint GetRetainerItemQuantity(uint itemId, FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags flags);
    bool TrySetQuantity(uint quantity);
    bool TryCloseRetainerInventory();
    bool TrySelectQuit();
    bool TryCloseRetainerList();
}

public sealed unsafe class RetainerGameUi : IRetainerGameUi
{
    private static readonly string[] RetainerInventoryAddons =
    {
        "RetainerGrid0", "RetainerGrid1", "RetainerGrid2", "RetainerGrid3", "RetainerGrid4",
        "RetainerCrystalGrid",
    };

    private static readonly InventoryType[] RetainerContainers =
    {
        InventoryType.RetainerPage1, InventoryType.RetainerPage2, InventoryType.RetainerPage3,
        InventoryType.RetainerPage4, InventoryType.RetainerPage5, InventoryType.RetainerPage6,
        InventoryType.RetainerPage7, InventoryType.RetainerCrystals,
    };

    private readonly IObjectTable _objects;
    private readonly ICondition _condition;
    private readonly IGameGui _gameGui;
    private readonly IDataManager _dataManager;
    private readonly IPluginLog _log;

    private string? _bellName;
    private string? _entrustText;
    private string? _quitText;
    private string? _retrieveAllText;
    private string? _retrieveQuantityText;

    public RetainerGameUi(IObjectTable objects, ICondition condition, IGameGui gameGui,
        IDataManager dataManager, IPluginLog log)
    {
        _objects = objects;
        _condition = condition;
        _gameGui = gameGui;
        _dataManager = dataManager;
        _log = log;
    }

    private string BellName => _bellName ??= _dataManager.GetExcelSheet<EObjName>().GetRow(2000401).Singular.ExtractText();
    private string EntrustText => _entrustText ??= _dataManager.GetExcelSheet<Addon>().GetRow(2378).Text.ExtractText();
    private string QuitText => _quitText ??= _dataManager.GetExcelSheet<Addon>().GetRow(2383).Text.ExtractText();
    private string RetrieveAllText => _retrieveAllText ??= _dataManager.GetExcelSheet<Addon>().GetRow(98).Text.ExtractText();
    private string RetrieveQuantityText => _retrieveQuantityText ??= _dataManager.GetExcelSheet<Addon>().GetRow(773).Text.ExtractText();

    public bool IsOccupiedAtBell => _condition[ConditionFlag.OccupiedSummoningBell];

    public bool IsRetainerListReady => TryGetReadyAddon("RetainerList", out _);

    public bool IsRetainerInventoryReady
    {
        get
        {
            if (!IsRetainerAgentActive())
                return false;

            // These are the client addons that actually render a retainer's bags.  They are used
            // by AutoRetainer's tested retrieval path; the generic Inventory addon is not a
            // reliable indication that the retainer inventory is available.
            foreach (var name in RetainerInventoryAddons)
            {
                if (TryGetReadyAddon(name, out _))
                    return true;
            }

            return false;
        }
    }

    public bool TryInteractWithNearestBell()
    {
        if (IsOccupiedAtBell)
            return true;

        var player = _objects.LocalPlayer;
        if (player == null)
            return false;

        foreach (var obj in _objects)
        {
            if (obj.ObjectKind != ObjectKind.HousingEventObject && obj.ObjectKind != ObjectKind.EventObj)
                continue;
            if (!obj.IsTargetable)
                continue;
            var name = obj.Name.TextValue;
            if (!string.Equals(name, BellName, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(name, "リテイナーベル", StringComparison.Ordinal))
                continue;
            if (Vector3.Distance(player.Position, obj.Position) > GetInteractionDistance(obj.ObjectKind))
                continue;

            TargetSystem.Instance()->InteractWithObject(
                (FFXIVClientStructs.FFXIV.Client.Game.Object.GameObject*)obj.Address, false);
            return true;
        }

        return false;
    }

    public bool TryAdvanceBellTalk()
    {
        if (!TryGetReadyAddon("Talk", out var talk))
            return false;

        // The summoning bell's initial text window is advanced with its empty callback.
        talk->FireCallback(0, null, true);
        return true;
    }

    public bool TrySelectRetainer(ulong retainerId)
    {
        if (!TryGetReadyAddon("RetainerList", out var addon))
            return false;

        var manager = RetainerManager.Instance();
        if (manager == null)
            return false;

        for (uint index = 0; index < manager->GetRetainerCount(); index++)
        {
            var retainer = manager->GetRetainerBySortedIndex(index);
            if (retainer == null || retainer->RetainerId != retainerId || !retainer->Available)
                continue;

            var values = stackalloc AtkValue[4];
            values[0] = new AtkValue { Type = AtkValueType.Int, Int = 2 };
            values[1] = new AtkValue { Type = AtkValueType.UInt, UInt = index };
            values[2] = new AtkValue { Type = 0, Int = 0 };
            values[3] = new AtkValue { Type = 0, Int = 0 };
            addon->FireCallback(4, values, true);
            return true;
        }

        return false;
    }

    public bool TrySelectEntrustOrWithdraw() => TrySelectStringEntry(EntrustText);

    public bool TrySelectQuit() => TrySelectStringEntry(QuitText);

    public bool TryRetrieve(uint itemId, InventoryItem.ItemFlags flags, uint quantity,
        out bool expectsQuantityInput, out uint willRetrieve)
    {
        expectsQuantityInput = false;
        willRetrieve = 0;
        if (!IsRetainerInventoryReady)
            return false;
        if (!TryFindRetainerSlot(itemId, flags, out var container, out var slot, out var stack))
            return false;

        var agent = AgentInventoryContext.Instance();
        var retainerAgent = AgentModule.Instance()->GetAgentByInternalId(AgentId.Retainer);
        if (agent == null || retainerAgent == null || !retainerAgent->IsAgentActive())
            return false;

        // Opening the native context menu is asynchronous.  The first call opens it and the next
        // framework update selects the matching native entry once the addon is ready.
        if (!TryGetReadyAddon("ContextMenu", out var contextMenu))
        {
            agent->OpenForItemSlot(container, slot, 0, retainerAgent->GetAddonId());
            return false;
        }

        var wantsPartial = stack > quantity;
        var entryIndex = FindContextEntry(agent, wantsPartial ? RetrieveQuantityText : RetrieveAllText);
        if (entryIndex < 0 && wantsPartial)
        {
            entryIndex = FindContextEntry(agent, RetrieveAllText);
            wantsPartial = false;
        }

        if (entryIndex < 0)
        {
            contextMenu->FireCallback(0, null, true);
            return false;
        }

        // Match Artisan's retainer context callback. The menu index is derived from the retainer
        // agent's string event parameters, rather than from UI node offsets that vary by client UI.
        var values = stackalloc AtkValue[6];
        values[0] = new AtkValue { Type = AtkValueType.Int, Int = 0 };
        values[1] = new AtkValue { Type = AtkValueType.Int, Int = entryIndex };
        values[2] = new AtkValue { Type = AtkValueType.Int, Int = 0 };
        values[3] = new AtkValue { Type = AtkValueType.Int, Int = 0 };
        values[4] = new AtkValue { Type = AtkValueType.Int, Int = 0 };
        values[5] = new AtkValue { Type = AtkValueType.Int, Int = 0 };
        contextMenu->FireCallback(6, values, true);

        expectsQuantityInput = wantsPartial;
        willRetrieve = wantsPartial ? quantity : stack;
        return true;
    }

    public uint GetRetainerItemQuantity(uint itemId, InventoryItem.ItemFlags flags)
    {
        var manager = InventoryManager.Instance();
        if (manager == null)
            return 0;

        var quantity = 0u;
        foreach (var candidate in RetainerContainers)
        {
            var inventory = manager->GetInventoryContainer(candidate);
            if (inventory == null)
                continue;

            for (var index = 0; index < inventory->Size; index++)
            {
                var item = inventory->GetInventorySlot(index);
                if (item != null && item->ItemId == itemId && item->Quantity > 0 && MatchesFlags(item->Flags, flags))
                    quantity += (uint)item->Quantity;
            }
        }

        return quantity;
    }

    public bool TrySetQuantity(uint quantity)
    {
        if (!TryGetReadyAddon("InputNumeric", out var numeric))
            return false;

        var maximum = numeric->AtkValues[3].UInt;
        var values = stackalloc AtkValue[1];
        values[0] = new AtkValue
        {
            Type = AtkValueType.Int,
            Int = (int)Math.Clamp(quantity, 1u, maximum),
        };
        numeric->FireCallback(1, values, true);
        return true;
    }

    public bool TryCloseRetainerInventory()
    {
        var retainerAgent = AgentModule.Instance()->GetAgentByInternalId(AgentId.Retainer);
        if (retainerAgent == null || !retainerAgent->IsAgentActive())
            return true;
        retainerAgent->Hide();
        return true;
    }

    public bool TryCloseRetainerList()
    {
        if (!TryGetReadyAddon("RetainerList", out var addon))
            return false;

        var values = stackalloc AtkValue[1];
        values[0] = new AtkValue { Type = AtkValueType.Int, Int = -1 };
        addon->FireCallback(1, values, true);
        return true;
    }

    private bool TrySelectStringEntry(string entryText)
    {
        if (!TryGetReadyAddon("SelectString", out var addon))
            return false;

        var selectString = (AddonSelectString*)addon;
        var count = selectString->PopupMenu.PopupMenu.EntryCount;
        for (var index = 0; index < count; index++)
        {
            var entryPointer = selectString->PopupMenu.PopupMenu.EntryNames[index].Value;
            if (entryPointer == null)
                continue;
            var entry = MemoryHelper.ReadSeStringNullTerminated((nint)entryPointer).TextValue;
            if (!entry.StartsWith(entryText, StringComparison.Ordinal))
                continue;

            var values = stackalloc AtkValue[1];
            values[0] = new AtkValue { Type = AtkValueType.Int, Int = index };
            addon->FireCallback(1, values, true);
            return true;
        }

        return false;
    }

    private static int FindContextEntry(AgentInventoryContext* agent, string expected)
    {
        var menuIndex = 0;
        foreach (var value in agent->EventParams)
        {
            if (value.Type != AtkValueType.String || value.String.Value == null)
                continue;
            var label = MemoryHelper.ReadSeStringNullTerminated((nint)value.String.Value).TextValue;
            if (string.Equals(label, expected, StringComparison.Ordinal))
                return menuIndex;
            menuIndex++;
        }

        return -1;
    }

    private static bool IsRetainerAgentActive()
    {
        var agent = AgentModule.Instance()->GetAgentByInternalId(AgentId.Retainer);
        return agent != null && agent->IsAgentActive();
    }

    private static bool TryFindRetainerSlot(uint itemId, InventoryItem.ItemFlags flags, out InventoryType container,
        out int slot, out uint stack)
    {
        var manager = InventoryManager.Instance();
        foreach (var candidate in RetainerContainers)
        {
            var inventory = manager->GetInventoryContainer(candidate);
            if (inventory == null)
                continue;
            for (var index = 0; index < inventory->Size; index++)
            {
                var item = inventory->GetInventorySlot(index);
                if (item == null || item->ItemId != itemId || item->Quantity == 0 || !MatchesFlags(item->Flags, flags))
                    continue;
                container = candidate;
                slot = index;
                stack = (uint)item->Quantity;
                return true;
            }
        }

        container = default;
        slot = -1;
        stack = 0;
        return false;
    }

    private static bool MatchesFlags(InventoryItem.ItemFlags actual, InventoryItem.ItemFlags required)
    {
        if (required.HasFlag(InventoryItem.ItemFlags.Collectable))
            return actual.HasFlag(InventoryItem.ItemFlags.Collectable);
        if (required.HasFlag(InventoryItem.ItemFlags.HighQuality))
            return actual.HasFlag(InventoryItem.ItemFlags.HighQuality) && !actual.HasFlag(InventoryItem.ItemFlags.Collectable);
        return !actual.HasFlag(InventoryItem.ItemFlags.HighQuality) && !actual.HasFlag(InventoryItem.ItemFlags.Collectable);
    }

    private static float GetInteractionDistance(ObjectKind objectKind)
    {
        return objectKind == ObjectKind.HousingEventObject ? 6.5f : 4.6f;
    }

    private bool TryGetReadyAddon(string name, out AtkUnitBase* addon)
    {
        addon = null;
        var pointer = _gameGui.GetAddonByName(name, 1);
        if (pointer == IntPtr.Zero)
            return false;
        var unitBase = (AtkUnitBase*)pointer.Address;
        if (unitBase == null || !unitBase->IsReady || !unitBase->IsVisible)
            return false;
        addon = unitBase;
        return true;
    }
}
