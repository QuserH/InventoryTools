using System;
using LuminaSupplemental.Excel.Model;
using InventoryTools.Localization;

namespace InventoryTools.Localizers;

public class RelicWeaponTypeLocalizer : ILocalizer<RelicWeaponType>
{
    public string Format(RelicWeaponType relicWeaponType)
    {
        switch (relicWeaponType)
        {
            case RelicWeaponType.Unknown:
                return "未知";
            case RelicWeaponType.ZodiacBase:
                return "上古武器";
            case RelicWeaponType.ZodiacZenith:
                return "上古武器·天极";
            case RelicWeaponType.ZodiacAtma:
                return "上古武器·魂晶";
            case RelicWeaponType.ZodiacAnimus:
                return "上古武器·魂灵";
            case RelicWeaponType.ZodiacNovus:
                return "上古武器·新星";
            case RelicWeaponType.ZodiacNexus:
                return "上古武器·镇魂";
            case RelicWeaponType.ZodiacZodiac:
                return "黄道武器";
            case RelicWeaponType.ZodiacZeta:
                return "黄道武器·本我";
            case RelicWeaponType.AnimaAnimated:
                return "元灵武器";
            case RelicWeaponType.AnimaAwoken:
                return "元灵武器·觉醒";
            case RelicWeaponType.AnimaAnima:
                return "新元灵武器";
            case RelicWeaponType.AnimaHyperconductive:
                return "元灵武器·超导";
            case RelicWeaponType.AnimaReconditioned:
                return "百炼成钢的元灵武器";
            case RelicWeaponType.AnimaSharpened:
                return "元灵武器·灵慧";
            case RelicWeaponType.AnimaComplete:
                return "真元灵武器";
            case RelicWeaponType.AnimaLux:
                return "真元灵武器·灵光";
            case RelicWeaponType.EurekanAntiquated:
                return "禁地兵装";
            case RelicWeaponType.EurekanBase:
                return "禁地兵装";
            case RelicWeaponType.EurekanBase1:
                return "禁地兵装+1";
            case RelicWeaponType.EurekanBase2:
                return "禁地兵装+2";
            case RelicWeaponType.EurekanAnemos:
                return "禁地兵装·常风";
            case RelicWeaponType.EurekanPagos:
                return "禁地兵装·恒冰";
            case RelicWeaponType.EurekanPagos1:
                return "禁地兵装·恒冰+1";
            case RelicWeaponType.EurekanElemental:
                return "禁地兵装·元素";
            case RelicWeaponType.EurekanElemental1:
                return "禁地兵装·元素+1";
            case RelicWeaponType.EurekanElemental2:
                return "禁地兵装·元素+2";
            case RelicWeaponType.EurekanPyros:
                return "禁地兵装·涌火";
            case RelicWeaponType.EurekanHydatos:
                return "禁地兵装·丰水";
            case RelicWeaponType.EurekanHydatos1:
                return "禁地兵装·丰水+1";
            case RelicWeaponType.EurekanBaseEureka:
                return "新禁地兵装";
            case RelicWeaponType.EurekanEureka:
                return "禁地兵装最终形态";
            case RelicWeaponType.EurekanPhyseos:
                return "禁地兵装·改装";
            case RelicWeaponType.ResistanceResistance:
                return "义军武器";
            case RelicWeaponType.ResistanceAugmentedResistance:
                return "义军武器+1";
            case RelicWeaponType.ResistanceRecollection:
                return "义军武器·回忆";
            case RelicWeaponType.ResistanceLawsOrder:
                return "义军武器·裁决";
            case RelicWeaponType.ResistanceAugmentedLawsOrder:
                return "义军武器·裁决+1";
            case RelicWeaponType.ResistanceBlades:
                return "义军武器·女王";
            case RelicWeaponType.MandervilleManderville:
                return "曼德维尔武器";
            case RelicWeaponType.MandervilleAmazing:
                return "曼德维尔武器·惊异";
            case RelicWeaponType.MandervilleMajestic:
                return "曼德维尔武器·威严";
            case RelicWeaponType.MandervilleMandervillous:
                return "曼德维尔武器·盈满";
            case RelicWeaponType.PhantomPenumbrae:
                return "幻境武器";
            case RelicWeaponType.PhantomUmbrae:
                return "幻境武器+1";
            case RelicWeaponType.PhantomObscurum:
                return "幻境武器+2";
        }

        return LocalizationService.Ui(relicWeaponType.ToString());
    }
}
