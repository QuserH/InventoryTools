using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class PatchFilter : StringFilter
{
    public override string Key { get; set; } = "PatchFilter";
    public override string Name { get; set; } = "版本";
    public override string HelpText { get; set; } = "物品加入游戏的版本。";
    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Basic;

    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return FilterItem(configuration, item.Item);
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        var currentValue = CurrentValue(configuration);
        if (!string.IsNullOrEmpty(currentValue))
        {
            if (item.Patch.PassesFilter(currentValue.ToLower()))
            {
                return true;
            }

            return false;
        }
        return true;
    }

    public PatchFilter(ILogger<PatchFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
        ShowOperatorTooltip = true;
    }
}