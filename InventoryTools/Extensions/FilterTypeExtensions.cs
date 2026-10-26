using InventoryTools.Logic;
using InventoryTools.Localization;

namespace InventoryTools.Extensions;

public static class FilterTypeExtensions
{
    public static string FormattedName(this FilterType filterType)
    {
        return filterType switch
        {
            FilterType.None => "无",
            FilterType.SearchFilter => "搜索列表",
            FilterType.SortingFilter => "排序列表",
            FilterType.GameItemFilter => "游戏物品列表",
            FilterType.CraftFilter => "制作列表",
            FilterType.HistoryFilter => "历史记录列表",
            FilterType.CuratedList => "精选列表",
            _ => "未知"
        };
    }
}