using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class HistoryChangeAmountColumn : TextColumn
{
    public HistoryChangeAmountColumn(ILogger<HistoryChangeAmountColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
    public override ColumnCategory ColumnCategory => ColumnCategory.History;

    public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        if (searchResult.InventoryChange != null)
        {
            return searchResult.InventoryChange.GetFormattedAmount().ToString();
        }

        return null;
    }

    public override string Name { get; set; } = "历史事件数量";
    public override string RenderName => "数量";
    public override float Width { get; set; } = 100;
    public override string HelpText { get; set; } = "变动数量（如适用）";
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    public override FilterType AvailableIn { get; } = Logic.FilterType.HistoryFilter;
    public override FilterType DefaultIn => Logic.FilterType.HistoryFilter;
}