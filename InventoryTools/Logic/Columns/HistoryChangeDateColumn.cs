using System;
using System.Globalization;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class HistoryChangeDateColumn : DateTimeColumn
{
    public HistoryChangeDateColumn(ILogger<HistoryChangeDateColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
    public override ColumnCategory ColumnCategory => ColumnCategory.History;
    public override DateTime? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        if (searchResult.InventoryChange != null)
        {
            return searchResult.InventoryChange.ChangeDate;
        }

        return null;
    }

    public override string CsvExport(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return CurrentValue(columnConfiguration, searchResult)?.ToString(CultureInfo.InvariantCulture) ?? "";
    }

    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("History Event Date/Time"));
    public override string RenderName => LocalizationService.Ui(LocalizationService.Ui("Date/Time"));
    public override float Width { get; set; } = 50;
    public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("When did the historical inventory event happen?"));
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    public override FilterType AvailableIn { get; } = Logic.FilterType.HistoryFilter;
    public override FilterType DefaultIn => Logic.FilterType.HistoryFilter;
}