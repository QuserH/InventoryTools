using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Localization;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters
{
    public class FilterORFilter : BooleanFilter
    {
        public override string Key { get; set; } = "ORFilter";
        public override string Name { get; set; } = "筛选物品时使用或条件。";

        public override string HelpText { get; set; } =
            "筛选物品时每个筛选集使用与条件缩小可用物品列表，可使用或条件代替与条件";

        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Advanced;



        public override bool? CurrentValue(FilterConfiguration configuration)
        {
            return configuration.UseORFiltering;
        }

        public override void UpdateFilterConfiguration(FilterConfiguration configuration, bool? newValue)
        {
            configuration.UseORFiltering = newValue;
        }

        public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
        {
            return null;
        }

        public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
        {
            return null;
        }

        public FilterORFilter(ILogger<FilterORFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}