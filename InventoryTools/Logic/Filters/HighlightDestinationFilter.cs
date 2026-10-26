using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Localization;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters
{
    public class HighlightDestinationFilter : BooleanFilter
    {
        public override string Key { get; set; } = "HighlightDestination";
        public override string Name { get; set; } = "高亮目标中的重复物品？";
        public override string HelpText { get; set; } =
            "目标背包中匹配的物品是否高亮？";
        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Display;
        public override FilterType AvailableIn { get; set; } = FilterType.SortingFilter;
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
            return configuration.HighlightDestination;
        }

        public override void UpdateFilterConfiguration(FilterConfiguration configuration, bool? newValue)
        {
            configuration.HighlightDestination = newValue;
        }

        public HighlightDestinationFilter(ILogger<HighlightDestinationFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}