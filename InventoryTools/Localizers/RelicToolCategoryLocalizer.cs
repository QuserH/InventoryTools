using System;
using LuminaSupplemental.Excel.Model;
using InventoryTools.Localization;

namespace InventoryTools.Localizers;

public class RelicToolCategoryLocalizer : ILocalizer<RelicToolCategory>
{
    public string Format(RelicToolCategory instance)
    {
        switch (instance)
        {
            case RelicToolCategory.Mastercraft:
                return LocalizationService.Ui("Mastercraft Tools");
            case RelicToolCategory.Skysteel:
                return LocalizationService.Ui("Skysteel Tools");
            case RelicToolCategory.Resplendent:
                return LocalizationService.Ui("Resplendent Tools");
            case RelicToolCategory.Splendorous:
                return LocalizationService.Ui("Splendorous Tools");
            case RelicToolCategory.Cosmic:
                return LocalizationService.Ui("Cosmic Tools");
        }
        return instance.ToString();
    }
}