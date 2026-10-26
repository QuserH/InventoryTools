using System.Numerics;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Localization;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters
{
    public class TabHighlightColorFilter : ColorFilter
    {
        public override FilterType AvailableIn { get; set; } =
            FilterType.SearchFilter | FilterType.CraftFilter | FilterType.SortingFilter | FilterType.GameItemFilter | FilterType.HistoryFilter | FilterType.CuratedList;
        public override string Key { get; set; } = "TabHighlightColor";
        public override string Name { get; set; } = "标签高亮颜色";

        public override string HelpText { get; set; } =
            "此筛选器高亮标签页（包含筛选物品的标签页）的颜色。未覆盖时使用标签页高亮颜色。";

        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Display;

        public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
        {
            return null;
        }

        public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
        {
            return null;
        }

        public override Vector4? CurrentValue(FilterConfiguration configuration)
        {
            return configuration.TabHighlightColor;
        }

        public override void UpdateFilterConfiguration(FilterConfiguration configuration, Vector4? newValue)
        {
            configuration.TabHighlightColor = newValue;
        }

        public TabHighlightColorFilter(ILogger<TabHighlightColorFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}