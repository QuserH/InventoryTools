using System;
using InventoryTools.Logic.Filters;
using InventoryTools.Localization;

namespace InventoryTools.Extensions;

public static class FilterCategoryExtensions
{
    public static string FormattedName(this FilterCategory filterCategory)
    {
        return filterCategory switch
        {
            FilterCategory.Basic => "基础",
            FilterCategory.Acquisition => "获取来源",
            FilterCategory.Crafting => "制作",
            FilterCategory.Gathering => "采集",
            FilterCategory.Searching => "搜索中",
            FilterCategory.Market => "市场",
            FilterCategory.Display => "显示",
            FilterCategory.Inventories => "物品栏",
            FilterCategory.Columns => "列",
            FilterCategory.Advanced => "高级",
            FilterCategory.CraftColumns => "制作列",
            FilterCategory.IngredientSourcing => "素材获取方式",
            FilterCategory.ZonePreference => "区域偏好",
            FilterCategory.WorldPricePreference => "服务器价格偏好",
            FilterCategory.Sources => "来源",
            FilterCategory.Uses => "用途",
            FilterCategory.SourceCategories => "来源（分类）",
            FilterCategory.UseCategories => "用途（分类）",
            FilterCategory.Settings => "设置",
            FilterCategory.Stats => "属性",
            FilterCategory.CompletionTracking => "完成追踪",
            FilterCategory.ItemIngredientOverrides => "物品素材覆盖",
            FilterCategory.Notifications => "通知",
            _ => LocalizationService.Ui(filterCategory.ToString().ToSentence())
        };
    }
}