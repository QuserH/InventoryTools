using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class RetainerMarketPriceColumn : GilColumn
    {
        public RetainerMarketPriceColumn(ILogger<RetainerMarketPriceColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Inventory;
        public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.InventoryItem != null)
            {
                return (int)searchResult.InventoryItem.RetainerMarketPrice;
            }

            return null;
        }
        public override string Name { get; set; } = "雇员出售单价";
        public override string RenderName => "雇员单价";
        public override float Width { get; set; } = 100;

        public override string HelpText { get; set; } =
            "若物品在市场出售，此为挂单单价。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}