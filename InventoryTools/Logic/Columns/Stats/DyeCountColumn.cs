using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns.Stats;

public class DyeCountColumn : IntegerColumn
{
    public DyeCountColumn(ILogger<DyeCountColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override string Name { get; set; } = "染色次数";
    public override float Width { get; set; } = 90;
    public override string HelpText { get; set; } = "物品已染色或可支持的染色次数。";
    public override ColumnCategory ColumnCategory { get; } = ColumnCategory.Stats;
    public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return searchResult.InventoryItem?.DyeCount ?? searchResult.Item.Base.DyeCount;
    }

    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
}