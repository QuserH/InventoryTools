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

        public override string Name { get; set; } = "是否可分解？";
        public override float Width { get; set; } = 100;
        public override string HelpText { get; set; } = "该物品是否可分解？";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
    }
}