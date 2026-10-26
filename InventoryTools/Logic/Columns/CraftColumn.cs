using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class CraftColumn : CheckboxColumn
    {
        public CraftColumn(ILogger<CraftColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.CanBeCrafted;
        }

        public override string Name { get; set; } = "是否可制作？";
        public override float Width { get; set; } = 125.0f;
        public override string HelpText { get; set; } = "该物品是否可制作？";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
        public override FilterType DefaultIn => Logic.FilterType.GameItemFilter;
    }
}