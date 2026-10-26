using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class SpiritbondColumn : TextColumn
    {
        public SpiritbondColumn(ILogger<SpiritbondColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.InventoryItem != null)
            {
                return searchResult.InventoryItem.ActualSpiritbond + "%%";
            }

            return null;
        }
        public override FilterType AvailableIn => Logic.FilterType.SearchFilter | Logic.FilterType.SortingFilter;
        public override string Name { get; set; } = "精炼度";
        public override float Width { get; set; } = 90.0f;
        public override string HelpText { get; set; } = "显示物品的精炼度百分比。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}