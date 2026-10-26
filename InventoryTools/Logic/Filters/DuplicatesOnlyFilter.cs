using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Localization;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters
{
    public class DuplicatesOnlyFilter : BooleanFilter
    {
        public override string Key { get; set; } = "DuplicatesOnly";
        public override string Name { get; set; } = "仅重复？";

        public override string HelpText { get; set; } =
            "是否筛除未同时出现在来源和目标中的物品？";

        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Searching;
        public override FilterType AvailableIn { get; set; } = FilterType.SortingFilter | FilterType.SearchFilter;
        
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
            return configuration.DuplicatesOnly;
        }

        public override void UpdateFilterConfiguration(FilterConfiguration configuration, bool? newValue)
        {
            configuration.DuplicatesOnly = newValue;
        }

        public DuplicatesOnlyFilter(ILogger<DuplicatesOnlyFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}