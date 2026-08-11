using System;
using System.Collections.Generic;
using System.Linq;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Enums;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using static FFXIVClientStructs.FFXIV.Client.Game.InventoryItem;

namespace InventoryTools.Services;

public sealed record RetainerRetrievalEntry(
    ulong RetainerId,
    uint ItemId,
    ItemFlags Flags,
    uint Quantity,
    IReadOnlyList<InventoryItem> Stacks);

public sealed class RetainerRetrievalPlan
{
    public IReadOnlyList<RetainerRetrievalEntry> Entries { get; }
    public uint TotalQuantity => Entries.Aggregate(0u, (total, entry) => total + entry.Quantity);
    public bool IsEmpty => Entries.Count == 0;

    public RetainerRetrievalPlan(IReadOnlyList<RetainerRetrievalEntry> entries)
    {
        Entries = entries;
    }
}

/// <summary>
/// Builds a deterministic, cache-only plan for the retainer inventories that belong to the active character.
/// This class deliberately has no game UI side effects and can be tested without a running game client.
/// Entries are emitted grouped per retainer so the automation can drain one retainer before switching.
/// </summary>
public sealed class RetainerRetrievalPlanner
{
    private readonly IInventoryMonitor _inventoryMonitor;
    private readonly ICharacterMonitor _characterMonitor;

    public RetainerRetrievalPlanner(IInventoryMonitor inventoryMonitor, ICharacterMonitor characterMonitor)
    {
        _inventoryMonitor = inventoryMonitor;
        _characterMonitor = characterMonitor;
    }

    public RetainerRetrievalPlan Build(CraftList craftList)
    {
        var required = new Dictionary<(uint ItemId, ItemFlags Flags), uint>();
        foreach (var item in craftList.GetFlattenedMaterials())
        {
            if (item.QuantityWillRetrieve == 0)
                continue;
            var key = (item.ItemId, RequiredFlags(item));
            required.TryAdd(key, 0);
            required[key] += item.QuantityWillRetrieve;
        }

        if (required.Count == 0 || !_characterMonitor.IsLoggedIn)
            return new RetainerRetrievalPlan(Array.Empty<RetainerRetrievalEntry>());

        var entries = new List<RetainerRetrievalEntry>();
        foreach (var retainer in _characterMonitor.GetRetainerCharacters(_characterMonitor.ActiveCharacterId))
        {
            if (!_inventoryMonitor.Inventories.TryGetValue(retainer.Key, out var inventory))
                continue;

            foreach (var requirement in required.ToArray())
            {
                var stacks = GetRetainerStacks(inventory, requirement.Key.ItemId, requirement.Key.Flags);
                if (stacks.Count == 0)
                    continue;

                var available = 0u;
                foreach (var stack in stacks)
                    available += stack.Quantity;

                var quantity = Math.Min(requirement.Value, available);
                if (quantity == 0)
                    continue;

                entries.Add(new RetainerRetrievalEntry(retainer.Key, requirement.Key.ItemId,
                    requirement.Key.Flags, quantity, stacks));
                required[requirement.Key] -= quantity;
                if (required[requirement.Key] == 0)
                    required.Remove(requirement.Key);
            }

            if (required.Count == 0)
                break;
        }

        return new RetainerRetrievalPlan(entries);
    }

    private static ItemFlags RequiredFlags(CraftItem item)
    {
        if (item.Flags.HasFlag(ItemFlags.Collectable))
            return ItemFlags.Collectable;
        if (item.Flags.HasFlag(ItemFlags.HighQuality))
            return ItemFlags.HighQuality;
        return ItemFlags.None;
    }

    private static List<InventoryItem> GetRetainerStacks(Inventory inventory, uint itemId, ItemFlags flags)
    {
        var stacks = new List<InventoryItem>();
        foreach (var item in inventory.GetItemsByCategory(InventoryCategory.RetainerBags))
        {
            if (item.ItemId == itemId && item.Quantity > 0 && MatchesFlags(item.Flags, flags))
                stacks.Add(item);
        }

        foreach (var item in inventory.GetItemsByType(InventoryType.RetainerCrystal))
        {
            if (item.ItemId == itemId && item.Quantity > 0 && MatchesFlags(item.Flags, flags))
                stacks.Add(item);
        }

        return stacks;
    }

    private static bool MatchesFlags(ItemFlags actual, ItemFlags required)
    {
        if (required == ItemFlags.Collectable)
            return actual.HasFlag(ItemFlags.Collectable);
        if (required == ItemFlags.HighQuality)
            return actual.HasFlag(ItemFlags.HighQuality) && !actual.HasFlag(ItemFlags.Collectable);
        return !actual.HasFlag(ItemFlags.HighQuality) && !actual.HasFlag(ItemFlags.Collectable);
    }
}
