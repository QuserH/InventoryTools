using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftReportPlaySoundFilter : GenericBooleanFilter
{
    public CraftReportPlaySoundFilter(ILogger<CraftReportPlaySoundFilter> logger, ImGuiService imGuiService) : base(
        "CraftReportPlaySound", LocalizationService.Ui("Play a sound when an item is complete?"),
        LocalizationService.Ui("Play a sound effect when an item in this list reaches its required quantity."),
        FilterCategory.Notifications, null, null, logger, imGuiService)
    {
        DefaultValue = false;
        AvailableIn = FilterType.CraftFilter;
    }
}