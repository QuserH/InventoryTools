using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Localization;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters
{
    public class TableCraftFreezeRowsFilter : IntegerFilter
    {
        public override string Key { get; set; } = "TableCraftFreezeRows";
        public override string Name { get; set; } = "冻结列";

        public override string HelpText { get; set; } =
            "从第1列开始冻结的列数（滚动时始终显示）。";

        public override FilterCategory FilterCategory { get; set; } = FilterCategory.CraftColumns;

        public override FilterType AvailableIn { get; set; } =
            FilterType.CraftFilter;

        public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
        {
            return null;
        }

        public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
        {
            return null;
        }

        public override void UpdateFilterConfiguration(FilterConfiguration configuration, int? newValue)
        {
            configuration.FreezeCraftColumns = newValue;
        }

        public override int? CurrentValue(FilterConfiguration configuration)
        {
            return configuration.FreezeCraftColumns;
        }

        public TableCraftFreezeRowsFilter(ILogger<TableCraftFreezeRowsFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}