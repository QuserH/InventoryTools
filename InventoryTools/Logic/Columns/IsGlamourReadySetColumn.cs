using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class IsGlamourReadySetColumn : CheckboxColumn
{
    public IsGlamourReadySetColumn(ILogger<IsGlamourReadySetColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;

    public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return searchResult.Item.HasUsesByType(ItemInfoType.GlamourReadySet);
    }

    public override string Name { get; set; } = "套装幻化？";
    public override float Width { get; set; } = 100;
    public override string HelpText { get; set; } = "该物品是否为一套幻化套装的组合形态？";
}