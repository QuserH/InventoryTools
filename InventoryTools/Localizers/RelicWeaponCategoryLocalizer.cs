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
                return  "上古武器";
            case RelicWeaponCategory.Anima:
                return  "元灵武器";
            case RelicWeaponCategory.Eurekan:
                return  "禁地兵装";
            case RelicWeaponCategory.Resistance:
                return  "义军武器";
            case RelicWeaponCategory.Manderville:
                return  "曼德维尔武器";
            case RelicWeaponCategory.Phantom:
                return  "幻境武器";
        }
        return instance.ToString();
    }
}
