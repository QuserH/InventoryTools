using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class VentureTypeColumn : TextColumn
{
    public VentureTypeColumn(ILogger<VentureTypeColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
    public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

    public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return string.Join(",", searchResult.Item.GetSourcesByCategory<ItemVentureSource>(ItemInfoCategory.AllVentures).Select(c => c.RetainerTaskRow.FormattedName));
    }
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Venture Type"));
    public override float Width { get; set; } = 100;
    public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("The type of ventures that the item can be acquired from"));
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
}