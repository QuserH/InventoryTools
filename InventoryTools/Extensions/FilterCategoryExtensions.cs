using System;
using InventoryTools.Logic.Filters;
using InventoryTools.Localization;

namespace InventoryTools.Extensions;

public static class FilterCategoryExtensions
{
    public static string FormattedName(this FilterCategory filterCategory)
    {
        return filterCategory switch
        {
            FilterCategory.Basic => LocalizationService.Ui("Basic"),
            FilterCategory.Acquisition => LocalizationService.Ui("Acquisition"),
            FilterCategory.Crafting => LocalizationService.Ui("Crafting"),
            FilterCategory.Gathering => LocalizationService.Ui("Gathering"),
            FilterCategory.Searching => LocalizationService.Ui("Searching"),
            FilterCategory.Market => LocalizationService.Ui("Market"),
            FilterCategory.Display => LocalizationService.Ui("Display"),
            FilterCategory.Inventories => LocalizationService.Ui("Inventories"),
            FilterCategory.Columns => LocalizationService.Ui("Columns"),
            FilterCategory.Advanced => LocalizationService.Ui("Advanced"),
            FilterCategory.CraftColumns => LocalizationService.Ui("Craft Columns"),
            FilterCategory.IngredientSourcing => LocalizationService.Ui("Ingredient Sourcing"),
            FilterCategory.ZonePreference => LocalizationService.Ui("Zone Preference"),
            FilterCategory.WorldPricePreference => LocalizationService.Ui("World Price Preference"),
            FilterCategory.Sources => LocalizationService.Ui("Sources"),
            FilterCategory.Uses => LocalizationService.Ui("Uses"),
            FilterCategory.SourceCategories => LocalizationService.Ui("Source (Categories)"),
            FilterCategory.UseCategories => LocalizationService.Ui("Use (Categories)"),
            FilterCategory.Settings => LocalizationService.Ui("Settings"),
            FilterCategory.Stats => LocalizationService.Ui("Stats"),
            FilterCategory.CompletionTracking => LocalizationService.Ui("Completion Tracking"),
            FilterCategory.ItemIngredientOverrides => LocalizationService.Ui("Item Ingredient Overrides"),
            FilterCategory.Notifications => LocalizationService.Ui("Notifications"),
            _ => LocalizationService.Ui(filterCategory.ToString().ToSentence())
        };
    }
}