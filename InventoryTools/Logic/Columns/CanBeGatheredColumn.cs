using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class CanBeGatheredColumn : CheckboxColumn
    {
        public CanBeGatheredColumn(ILogger<CanBeGatheredColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.ObtainedGathering || searchResult.Item.ObtainedFishing;
        }
        public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Is Gatherable?"));
        public override float Width { get; set; } = 80.0f;
        public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Can the item be gathered?"));
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
        public override FilterType DefaultIn => Logic.FilterType.GameItemFilter;
    }
}