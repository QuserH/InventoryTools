using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftReportTrackGatheringFilter : GenericBooleanFilter
{
    public CraftReportTrackGatheringFilter(ILogger<CraftReportTrackGatheringFilter> logger, ImGuiService imGuiService) :
        base(
            "CraftReportTrackGathering", "报告采集物品？",
            "采集属于此制作列表的物品时报告进度。", FilterCategory.Notifications,
            null, null, logger, imGuiService)
    {
        DefaultValue = true;
        AvailableIn = FilterType.CraftFilter;
    }
}

public class CraftReportTrackCraftingFilter : GenericBooleanFilter
{
    public CraftReportTrackCraftingFilter(ILogger<CraftReportTrackCraftingFilter> logger, ImGuiService imGuiService) :
        base(
            "CraftReportTrackCrafting", "报告制作物品？",
            "制作属于此制作列表的物品时报告进度。", FilterCategory.Notifications,
            null, null, logger, imGuiService)
    {
        DefaultValue = false;
        AvailableIn = FilterType.CraftFilter;
    }
}

public class CraftReportTrackShoppingFilter : GenericBooleanFilter
{
    public CraftReportTrackShoppingFilter(ILogger<CraftReportTrackShoppingFilter> logger, ImGuiService imGuiService) :
        base(
            "CraftReportTrackShopping", "报告商店购买物品？",
            "购买属于此制作列表的物品时报告进度。", FilterCategory.Notifications,
            null, null, logger, imGuiService)
    {
        DefaultValue = false;
        AvailableIn = FilterType.CraftFilter;
    }
}

public class CraftReportTrackCombatDropFilter : GenericBooleanFilter
{
    public CraftReportTrackCombatDropFilter(ILogger<CraftReportTrackCombatDropFilter> logger, ImGuiService imGuiService)
        : base(
            "CraftReportTrackCombatDrop", "报告战斗掉落？",
            "属于此制作列表的物品从战斗中掉落时报告进度。",
            FilterCategory.Notifications,
            null, null, logger, imGuiService)
    {
        DefaultValue = false;
        AvailableIn = FilterType.CraftFilter;
    }
}

public class CraftReportTrackMarketBoardFilter : GenericBooleanFilter
{
    public CraftReportTrackMarketBoardFilter(ILogger<CraftReportTrackMarketBoardFilter> logger,
        ImGuiService imGuiService) : base(
        "CraftReportTrackMarketBoard", "报告市场布告板购买？",
        "从市场布告板购买属于此制作列表的物品时报告进度。",
        FilterCategory.Notifications,
        null, null, logger, imGuiService)
    {
        DefaultValue = false;
        AvailableIn = FilterType.CraftFilter;
    }
}

public class CraftReportTrackOtherFilter : GenericBooleanFilter
{
    public CraftReportTrackOtherFilter(ILogger<CraftReportTrackOtherFilter> logger, ImGuiService imGuiService) : base(
        "CraftReportTrackOther", "报告其他获取？",
        "通过其他任何途径获得属于此制作列表的物品时报告进度。",
        FilterCategory.Notifications,
        null, null, logger, imGuiService)
    {
        DefaultValue = false;
        AvailableIn = FilterType.CraftFilter;
    }
}