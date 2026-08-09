using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class PatchColumn : DecimalColumn
{
    public PatchColumn(ILogger<PatchColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
    public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
    public override decimal? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return searchResult.Item.Patch;
    }
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Patch Added"));
    public override string RenderName => LocalizationService.Ui("Patch");
    public override float Width { get; set; } = 100;
    public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Shows the patch in which the item was added."));
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
}