using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftReverseListDisplayFilter : BooleanFilter
{
    public CraftReverseListDisplayFilter(ILogger<CraftReverseListDisplayFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override string Key { get; set; } = "CraftReverseListDisplay";
    public override string Name { get; set; } = "反转制作列表顺序？";

    public override string HelpText { get; set; } =
        "制作列表是否倒序显示？即成品是否从底部开始？（仅适用于制作显示模式为单表时）";

    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;

    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Display;
    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return null;
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        return null;
    }
}