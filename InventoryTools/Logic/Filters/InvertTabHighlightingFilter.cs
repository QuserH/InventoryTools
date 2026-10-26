using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Localization;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters
{
    public class InvertTabHighlightingFilter : BooleanFilter
    {
        public override FilterType AvailableIn { get; set; } =
            FilterType.SearchFilter | FilterType.CraftFilter | FilterType.SortingFilter | FilterType.GameItemFilter | FilterType.HistoryFilter | FilterType.CuratedList;
        public override string Key { get; set; } = "InvertTabHighlighting";
        public override string Name { get; set; } = "反转标签高亮？";
        public override string HelpText { get; set; } = "是否改为高亮不匹配筛选器的物品？设为不适用时将使用常规设置中的反转高亮选项。";
        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Display;

        public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
        {
            return null;
        }

        public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
        {
            return null;
        }

        public override bool? CurrentValue(FilterConfiguration configuration)
        {
            return configuration.InvertTabHighlighting;
        }

        public override void UpdateFilterConfiguration(FilterConfiguration configuration, bool? newValue)
        {
            configuration.InvertTabHighlighting = newValue;
        }

        public override bool HasValueSet(FilterConfiguration configuration)
        {
            return configuration.InvertTabHighlighting != null;
        }

        public InvertTabHighlightingFilter(ILogger<InvertTabHighlightingFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}