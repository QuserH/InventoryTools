using System;
using LuminaSupplemental.Excel.Model;
using InventoryTools.Localization;

namespace InventoryTools.Localizers;

public class RelicWeaponCategoryLocalizer : ILocalizer<RelicWeaponCategory>
{
    public string Format(RelicWeaponCategory instance)
    {
        switch (instance)
        {
            case RelicWeaponCategory.Zodiac:
                return  LocalizationService.Ui("Zodiac Weapons");
            case RelicWeaponCategory.Anima:
                return  LocalizationService.Ui("Anima Weapons");
            case RelicWeaponCategory.Eurekan:
                return  LocalizationService.Ui("Eurekan Weapons");
            case RelicWeaponCategory.Resistance:
                return  LocalizationService.Ui("Resistance Weapons");
            case RelicWeaponCategory.Manderville:
                return  LocalizationService.Ui("Manderville Weapons");
            case RelicWeaponCategory.Phantom:
                return  LocalizationService.Ui("Phantom Weapons");
        }
        return instance.ToString();
    }
}