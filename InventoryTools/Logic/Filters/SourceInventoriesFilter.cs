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
        public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Source - Inventories"));
        public override string HelpText { get; set; } =
            LocalizationService.Ui(LocalizationService.Ui("This is a list of source inventories to sort items from based on the filter configuration"));
        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Inventories;

        public override List<InventorySearchScope>? DefaultValue { get; set; } = null;
        public override FilterType AvailableIn { get; set; } = FilterType.SearchFilter | FilterType.SortingFilter | FilterType.HistoryFilter;

        public override string GetName(FilterConfiguration configuration)
        {
            switch (configuration.FilterType)
            {
                case FilterType.SearchFilter:
                case FilterType.HistoryFilter:
                    return LocalizationService.Ui("Search Inventories");
                case FilterType.SortingFilter:
                    return LocalizationService.Ui("Source Inventories");
            }

            return Name;
        }

        public override string GetHelpText(FilterConfiguration configuration)
        {
            switch (configuration.FilterType)
            {
                case FilterType.SearchFilter:
                    return
                        LocalizationService.Ui("Define which inventories you want to be searched, these will show up in the list. You can see which inventories have been found based on the scope configuration below.");
                case FilterType.HistoryFilter:
                    return
                        LocalizationService.Ui("Define the scope of inventories you want to see in the historical list of inventory changes.");
                case FilterType.SortingFilter:
                    return
                        LocalizationService.Ui("Define which inventories you want to be searched, the plugin will then attempt to show you where to put them in the destinations you have selected. You can see which inventories have been found based on the scope configuration below.");
            }

            return HelpText;
        }

        public override List<InventorySearchScope>? GenerateDefaultScope()
        {
            return null;
        }

    }
}