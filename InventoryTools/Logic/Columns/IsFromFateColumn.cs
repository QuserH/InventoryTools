using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class IsFromFateColumn : CheckboxColumn
{
    public IsFromFateColumn(ILogger<IsFromFateColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {

    }

    public override string Name { get; set; } = "来自危命任务？";
    public override float Width { get; set; } = 80;
    public override string HelpText { get; set; } = "该物品是否通过危命任务掉落/获得？";
    public override ColumnCategory ColumnCategory { get; } = ColumnCategory.Basic;
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;

    public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return searchResult.Item.HasSourcesByType(ItemInfoType.Fate);
    }
}