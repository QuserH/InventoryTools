using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters.Stats;

public class DyeCountFilter : StringFilter
{
    public DyeCountFilter(ILogger<DyeCountFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
        ShowOperatorTooltip = true;
    }

    public override string Key { get; set; } = "DyeCount";
    public override string Name { get; set; } = "染色次数";
    public override string HelpText { get; set; } = "该物品已染色或可支持的染色次数？";
    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Stats;
    public override FilterType AvailableIn { get; set; }  = FilterType.SearchFilter | FilterType.SortingFilter | FilterType.GameItemFilter | FilterType.HistoryFilter;
    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        var currentValue = CurrentValue(configuration);
        if (!string.IsNullOrEmpty(currentValue))
        {
            if (item.DyeCount.PassesFilter(currentValue.ToLower()))
            {
                return true;
            }

            return false;
        }
        return true;
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        var currentValue = CurrentValue(configuration);
        if (!string.IsNullOrEmpty(currentValue))
        {
            if (((int)item.Base.DyeCount).PassesFilter(currentValue.ToLower()))
            {
                return true;
            }

            return false;
        }
        return true;
    }
}