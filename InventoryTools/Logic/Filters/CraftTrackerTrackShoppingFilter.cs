using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftTrackerTrackShoppingFilter : BooleanFilter
{
    public CraftTrackerTrackShoppingFilter(ILogger<CraftTrackerTrackShoppingFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
    
    public override bool? DefaultValue { get; set; } = true;
    
    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    
    public override string Key { get; set; } = "CraftTrackerTrackShopping";
    public override string Name { get; set; } = "追踪商店购买？";
    
    public override string HelpText { get; set; } =
        "从商店购买且与制作列表中的成品匹配时，是否扣除该物品的数量？制作列表必须处于启用状态才会计入。";
    
    public override FilterCategory FilterCategory { get; set; } = FilterCategory.CompletionTracking;
    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return null;
    }
    
    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        return null;
    }
}