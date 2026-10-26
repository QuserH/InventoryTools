using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class CanBePurchasedColumn : CheckboxColumn
    {
        public CanBePurchasedColumn(ILogger<CanBePurchasedColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.HasSourcesByType(ItemInfoType.GilShop, ItemInfoType.CalamitySalvagerShop);
        }
        public override string Name { get; set; } = "是否可购买？";
        public override float Width { get; set; } = 70.0f;
        public override string HelpText { get; set; } = "该物品是否可用金币购买？";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
        public override FilterType DefaultIn => Logic.FilterType.GameItemFilter;
    }
}