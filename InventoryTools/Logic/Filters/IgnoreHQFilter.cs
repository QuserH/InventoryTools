using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Localization;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public class IgnoreHQFilter : BooleanFilter
{
    public override string Key { get; set; } = "IgnoreHQFilter";
    public override string Name { get; set; } = "忽略优质品筛选？";

    public override string HelpText { get; set; } =
        "整理时筛选器是否将HQ与NQ视为同种以便堆叠？此筛选器主要用于寻找可降低品质的物品。";

    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Advanced;
    public override FilterType AvailableIn { get; set; } = FilterType.SortingFilter;

    public override void UpdateFilterConfiguration(FilterConfiguration configuration, bool? newValue)
    {
        configuration.IgnoreHQWhenSorting = newValue;
    }

    public override bool? CurrentValue(FilterConfiguration configuration)
    {
        return configuration.IgnoreHQWhenSorting;
    }

    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return true;
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        return true;
    }

    public IgnoreHQFilter(ILogger<IgnoreHQFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}