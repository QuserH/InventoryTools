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
        var list = results.ToList();
        if (!mergeSameSource)
        {
            return list;
        }

        var rows = list.Where(r => r.InventoryItem != null).ToList();

        IEnumerable<SearchResult> merged = mergeNqHq
            ? rows.GroupBy(r => (r.ItemId, r.InventoryItem!.RetainerId, IsMarket(r))).Select(g => MergeGroup(g, true))
            : rows.GroupBy(r => (r.ItemId, r.Flags, r.InventoryItem!.RetainerId, IsMarket(r))).Select(g => MergeGroup(g, false));

        return merged.ToList();
    }

    private static bool IsMarket(SearchResult result)
    {
        return result.InventoryItem!.SortedContainer == InventoryType.RetainerMarket ||
               result.InventoryItem!.Container == InventoryType.RetainerMarket;
    }

    private static bool IsFreeCompany(SearchResult result)
    {
        return result.InventoryItem!.SortedCategory == InventoryCategory.FreeCompanyBags;
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
        merged.MergedContainsMarket = IsMarket(first);
        // The company-chest label is only shown when every combined stack lives in the Free
        // Company chest; mixed rows fall back to the container name of the first stack.
        merged.MergedContainsFreeCompany = list.All(IsFreeCompany);
        return merged;
    }

    private static readonly System.Reflection.MethodInfo CloneMethod =
        typeof(object).GetMethod("MemberwiseClone", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;

    private static InventoryItem CloneItem(InventoryItem item)
    {
        return (InventoryItem)CloneMethod.Invoke(item, null)!;
    }
}
