using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class IsStackableColumn : CheckboxColumn
    {
        public IsStackableColumn(ILogger<IsStackableColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }

        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.Base.StackSize > 1;
        }

        public override string Name { get; set; } = "是否可堆叠？";
        public override float Width { get; set; } = 100;
        public override string HelpText { get; set; } = "该物品是否可堆叠（一组可超过 1 个）？";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
    }
}
