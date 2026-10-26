using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class IsGCSupplyItemColumn : CheckboxColumn
{

    public IsGCSupplyItemColumn(ILogger<IsGCSupplyItemColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override string Name { get; set; } = "军需筹备品？";
    public override float Width { get; set; } = 80;
    public override string HelpText { get; set; } = "该物品是否用于大国防联军筹备任务？";
    public override ColumnCategory ColumnCategory { get; } = ColumnCategory.Basic;
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;

    public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return searchResult.Item.HasUsesByType(ItemInfoType.GCDailySupply);
    }
}