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
    bool IsRetainerActionMenuReady { get; }
    bool IsRetainerInventoryReady { get; }
    bool TryInteractWithNearestBell();
    bool TryAdvanceBellTalk();
    bool TrySelectRetainer(ulong retainerId);
    bool TrySelectEntrustOrWithdraw();
    bool TryRetrieve(uint itemId, FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags flags, uint quantity, out bool expectsQuantityInput, out uint willRetrieve);
    bool ContainsRetainerItem(uint itemId, FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags flags);
    uint GetRetainerItemQuantity(uint itemId, FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags flags);
    bool TrySetQuantity(uint quantity);
    bool TryConfirmTransfer();
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

    public bool IsRetainerActionMenuReady
    {
        get
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
                if (TextMatches(entry, EntrustText) || IsEntrustOrWithdrawEntry(entry))
                    return true;
            }

            return false;
        }
    }

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

        // A retainer can show Talk both after the bell is used and immediately after it is selected.
        // Talk does not handle the generic empty callback here.  Mirror the actual mouse sequence
        // used by Artisan/AutoRetainer so the game advances to RetainerList or SelectString.
        var evt = stackalloc AtkEvent[1];
        evt[0] = new AtkEvent
        {
            Listener = (AtkEventListener*)talk,
            Target = &AtkStage.Instance()->AtkEventTarget,
            State = new AtkEventState { StateFlags = (AtkEventStateFlags)132 },
        };
        var data = stackalloc AtkEventData[1];
        for (var index = 0; index < sizeof(AtkEventData); index++)
            ((byte*)data)[index] = 0;

        talk->ReceiveEvent(AtkEventType.MouseDown, 0, evt, data);
        talk->ReceiveEvent(AtkEventType.MouseClick, 0, evt, data);
        talk->ReceiveEvent(AtkEventType.MouseUp, 0, evt, data);
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

    public bool TrySelectEntrustOrWithdraw() => TrySelectStringEntry(EntrustText, IsEntrustOrWithdrawEntry);

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

        // Use the live native stack as the hard upper bound. The craft cache can still contain
        // an aggregate quantity from several retainers or a stale scan, but one context action
        // can only operate on this slot.
        var requested = Math.Min(quantity, stack);
        var wantsPartial = requested < stack;
        var entryIndex = FindContextEntry(contextMenu, wantsPartial ? RetrieveQuantityText : RetrieveAllText);

        if (entryIndex < 0)
        {
            // Never fall back to "retrieve all" for a partial request. A missing or shifted
            // context-menu index must retry safely instead of transferring the whole stack.
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
        willRetrieve = requested;
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

    public bool ContainsRetainerItem(uint itemId, InventoryItem.ItemFlags flags)
    {
        return TryFindRetainerSlot(itemId, flags, out _, out _, out _);
    }

    public bool TrySetQuantity(uint quantity)
    {
        if (!TryGetReadyAddon("InputNumeric", out var numeric))
            return false;

        var maximum = numeric->AtkValues[3].UInt;
        if (maximum == 0)
            return false;
        var values = stackalloc AtkValue[1];
        values[0] = new AtkValue
        {
            Type = AtkValueType.Int,
            Int = (int)Math.Clamp(quantity, 1u, maximum),
        };
        numeric->FireCallback(1, values, true);
        return true;
    }

    public bool TryConfirmTransfer()
    {
        if (!TryGetReadyAddon("RetainerItemTransferProgress", out var addon))
            return false;

        var button = addon->GetComponentButtonById(9);
        if (button == null || !button->IsEnabled || !button->AtkResNode->IsVisible())
            return false;

        var component = (AtkComponentBase*)button;
        var buttonNode = component->OwnerNode->AtkResNode;
        var evt = (AtkEvent*)buttonNode.AtkEventManager.Event;
        addon->ReceiveEvent(evt->State.EventType, (int)evt->Param, buttonNode.AtkEventManager.Event);
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

    private bool TrySelectStringEntry(string entryText, Func<string, bool>? fallbackMatch = null)
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
            if (!TextMatches(entry, entryText) && !(fallbackMatch?.Invoke(entry) ?? false))
                continue;

            var values = stackalloc AtkValue[1];
            values[0] = new AtkValue { Type = AtkValueType.Int, Int = index };
            addon->FireCallback(1, values, true);
            return true;
        }

        return false;
    }

    private bool IsEntrustOrWithdrawEntry(string entry)
    {
        var normalized = NormalizeMenuText(entry);
        return normalized.Contains("entrust", StringComparison.Ordinal) ||
               normalized.Contains("withdraw", StringComparison.Ordinal) ||
               normalized.Contains("itemtransfer", StringComparison.Ordinal) ||
               normalized.Contains("アイテム", StringComparison.Ordinal) ||
               normalized.Contains("道具", StringComparison.Ordinal) ||
               normalized.Contains("物品", StringComparison.Ordinal) ||
               normalized.Contains("寄存", StringComparison.Ordinal) ||
               normalized.Contains("取回", StringComparison.Ordinal);
    }

    private static bool TextMatches(string entry, string expected)
    {
        var normalizedEntry = NormalizeMenuText(entry);
        var normalizedExpected = NormalizeMenuText(expected);
        return normalizedEntry.StartsWith(normalizedExpected, StringComparison.Ordinal) ||
               normalizedExpected.StartsWith(normalizedEntry, StringComparison.Ordinal);
    }

    private static string NormalizeMenuText(string value)
    {
        Span<char> buffer = stackalloc char[value.Length];
        var length = 0;
        foreach (var character in value)
        {
            if (!char.IsWhiteSpace(character) && !char.IsPunctuation(character) && !char.IsControl(character))
                buffer[length++] = char.ToLowerInvariant(character);
        }

        return new string(buffer[..length]);
    }

    private static int FindContextEntry(AtkUnitBase* addon, string expected)
    {
        // ContextMenu stores its entry count in AtkValues[0] and each native entry label at
        // AtkValues[8 + index]. Reading the visible addon directly keeps the callback index
        // aligned with the entries the player sees; crystal menus expose labels as managed
        // strings, so both string types are accepted.
        var count = addon->AtkValues[0].UInt;
        for (var index = 0; index < count; index++)
        {
            var value = addon->AtkValues[8 + index];
            if ((value.Type != AtkValueType.String && value.Type != AtkValueType.ManagedString) || value.String.Value == null)
                continue;
            var label = MemoryHelper.ReadSeStringNullTerminated((nint)value.String.Value).TextValue;
            if (string.Equals(label, expected, StringComparison.Ordinal))
                return index;
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
