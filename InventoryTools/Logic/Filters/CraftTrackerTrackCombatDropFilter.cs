using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftTrackerTrackCombatDropFilter : BooleanFilter
{
    public CraftTrackerTrackCombatDropFilter(ILogger<CraftTrackerTrackCombatDropFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
    
    public override bool? DefaultValue { get; set; } = false;
    
    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    
    public override string Key { get; set; } = "CraftTrackerTrackCombatDrop";
    public override string Name { get; set; } = "追踪战斗掉落？";
    
    public override string HelpText { get; set; } =
        "战斗中掉落物品且与制作列表中的成品匹配时，是否扣除该物品的数量？制作列表必须处于启用状态才会计入。";
    
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