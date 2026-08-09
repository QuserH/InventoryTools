using InventoryTools.Logic;
using InventoryTools.Localization;

namespace InventoryTools.Extensions;

public static class FilterTypeExtensions
{
    public static string FormattedName(this FilterType filterType)
    {
        return filterType switch
        {
            FilterType.None => LocalizationService.Ui("None"),
            FilterType.SearchFilter => LocalizationService.Ui("Search List"),
            FilterType.SortingFilter => LocalizationService.Ui("Sort List"),
            FilterType.GameItemFilter => LocalizationService.Ui("Game Item List"),
            FilterType.CraftFilter => LocalizationService.Ui("Craft List"),
            FilterType.HistoryFilter => LocalizationService.Ui("History List"),
            FilterType.CuratedList => LocalizationService.Ui("Curated List"),
            _ => LocalizationService.Ui("Unknown")
        };
    }
}