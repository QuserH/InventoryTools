using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Misc;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class IsHousingItemColumn : CheckboxColumn
    {
        public IsHousingItemColumn(ILogger<IsHousingItemColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return Helpers.HousingCategoryIds.Contains(searchResult.Item.Base.ItemUICategory.RowId);
        }
        public override string Name { get; set; } = "是否为房屋物品？";
        public override string RenderName => "是否为房屋物品？";
        public override float Width { get; set; } = 100;
        public override string HelpText { get; set; } = "该物品是否为房屋物品？目前可能略有误差。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}