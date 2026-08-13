using System.Collections.Generic;
using System.Linq;
using CriticalCommonLib.Enums;
using CriticalCommonLib.Models;

namespace InventoryTools.Logic;

/// <summary>
/// Builds merged inventory rows for the craft list inventory panel. Rows are only combined when
/// the character, source (retainer) and item name all match; whether NQ and HQ are combined as
/// well is controlled by mergeNqHq. Merged rows hide the specific bag location but still report
/// when any of the combined stacks is listed on the market.
/// </summary>
public static class MergedSearchResults
{
    public static List<SearchResult> Apply(IEnumerable<SearchResult> results, bool mergeSameSource, bool mergeNqHq)
    {
        var rows = results.Where(r => r.InventoryItem != null).ToList();
        if (!mergeSameSource)
        {
            return rows;
        }

        IEnumerable<SearchResult> merged = mergeNqHq
            ? rows.GroupBy(r => (r.ItemId, r.InventoryItem!.RetainerId)).Select(g => MergeGroup(g, true))
            : rows.GroupBy(r => (r.ItemId, r.Flags, r.InventoryItem!.RetainerId)).Select(g => MergeGroup(g, false));

        return merged.ToList();
    }

    private static SearchResult MergeGroup(IEnumerable<SearchResult> rows, bool mergeNqHq)
    {
        var list = rows.ToList();
        var first = list[0];
        if (list.Count == 1)
        {
            return first;
        }

        var clone = CloneItem(first.InventoryItem!);
        clone.Quantity = (uint)list.Sum(r => r.InventoryItem!.Quantity);
        if (mergeNqHq)
        {
            clone.Flags = FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.None;
        }

        var merged = new SearchResult(clone) { IsMerged = true };
        merged.MergedContainsMarket = list.Any(r =>
            r.InventoryItem!.SortedContainer == InventoryType.RetainerMarket ||
            r.InventoryItem!.Container == InventoryType.RetainerMarket);
        return merged;
    }

    private static InventoryItem CloneItem(InventoryItem item)
    {
        var cloneMethod = typeof(object).GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        return (InventoryItem)cloneMethod!.Invoke(item, null)!;
    }
}
