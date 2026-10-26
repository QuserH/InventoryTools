using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Extensions;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using InventoryTools.Logic.Editors;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters
{
    public class SourceInventoriesFilter : InventoryScopeFilter
    {
        public SourceInventoriesFilter(ILogger<SourceInventoriesFilter> logger, InventoryScopePicker scopePicker, ImGuiService imGuiService) : base(scopePicker, logger, imGuiService)
        {
        }
        public override int LabelSize { get; set; } = 240;
        public override string Key { get; set; } = "SourceInventories";
        public override string Name { get; set; } = "来源背包";
        public override string HelpText { get; set; } =
            "基于筛选器配置的来源背包列表";
        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Inventories;

        public override List<InventorySearchScope>? DefaultValue { get; set; } = null;
        public override FilterType AvailableIn { get; set; } = FilterType.SearchFilter | FilterType.SortingFilter | FilterType.HistoryFilter;

        public override string GetName(FilterConfiguration configuration)
        {
            switch (configuration.FilterType)
            {
                case FilterType.SearchFilter:
                case FilterType.HistoryFilter:
                    return "搜索背包";
                case FilterType.SortingFilter:
                    return "来源背包";
            }

            return Name;
        }

        public override string GetHelpText(FilterConfiguration configuration)
        {
            switch (configuration.FilterType)
            {
                case FilterType.SearchFilter:
                    return
                        "定义要搜索的背包，这些将显示在列表中。你可根据下方范围配置查看已找到的背包。";
                case FilterType.HistoryFilter:
                    return
                        "定义要在背包变动历史列表中查看的背包范围。";
                case FilterType.SortingFilter:
                    return
                        "定义要搜索的背包，插件将尝试显示应将其放入所选目标的位置。你可根据下方范围配置查看已找到的背包。";
            }

            return HelpText;
        }

        public override List<InventorySearchScope>? GenerateDefaultScope()
        {
            return null;
        }

    }
}