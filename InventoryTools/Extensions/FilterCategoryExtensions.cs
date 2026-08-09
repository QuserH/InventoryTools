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
            FilterCategory.SourceCategories => LocalizationService.Ui("Source (Categories)"),
            FilterCategory.UseCategories => LocalizationService.Ui("Use (Categories)"),
            _ => filterCategory.ToString().ToSentence()
        };
    }
}