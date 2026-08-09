using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftReportCompletionOnlyFilter : GenericBooleanFilter
{
    public CraftReportCompletionOnlyFilter(ILogger<CraftReportCompletionOnlyFilter> logger, ImGuiService imGuiService) :
        base(
            "CraftReportCompletionOnly", LocalizationService.Ui("Only report when an item is complete?"),
            LocalizationService.Ui("Instead of reporting every acquisition, only print a message when an item reaches its required quantity."),
            FilterCategory.Notifications, null, null, logger, imGuiService)
    {
        DefaultValue = false;
        AvailableIn = FilterType.CraftFilter;
    }
}