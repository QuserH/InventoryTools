using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class CanBeDesynthedColumn : CheckboxColumn
    {
        public CanBeDesynthedColumn(ILogger<CanBeDesynthedColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Desynthesis;

        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.Base.Desynth != 0;
        }

        public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Is Desynthable?"));
        public override float Width { get; set; } = 100;
        public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Can the item by desynthed?"));
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
    }
}