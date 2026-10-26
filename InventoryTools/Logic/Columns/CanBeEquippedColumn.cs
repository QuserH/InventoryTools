using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class CanBeEquippedColumn : CheckboxColumn
{
    public CanBeEquippedColumn(ILogger<CanBeEquippedColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
    public override ColumnCategory ColumnCategory { get; } = ColumnCategory.Basic;
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;

    public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return searchResult.Item.Base.EquipSlotCategory.RowId != 0;
    }
    public override string Name { get; set; } = "是否可装备？";
    public override float Width { get; set; } = 100;
    public override string HelpText { get; set; } = "该物品是否可装备？";
}