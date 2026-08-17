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
                return LocalizationService.Ui("Unknown");
            case RelicWeaponType.ZodiacBase:
                return LocalizationService.Ui("Base");
            case RelicWeaponType.ZodiacZenith:
                return LocalizationService.Ui("Zenith");
            case RelicWeaponType.ZodiacAtma:
                return LocalizationService.Ui("Atma");
            case RelicWeaponType.ZodiacAnimus:
                return LocalizationService.Ui("Animus");
            case RelicWeaponType.ZodiacNovus:
                return LocalizationService.Ui("Novus");
            case RelicWeaponType.ZodiacNexus:
                return LocalizationService.Ui("Nexus");
            case RelicWeaponType.ZodiacZodiac:
                return LocalizationService.Ui("Zodiac");
            case RelicWeaponType.ZodiacZeta:
                return LocalizationService.Ui("Zeta");
            case RelicWeaponType.AnimaAnimated:
                return LocalizationService.Ui("Animated");
            case RelicWeaponType.AnimaAwoken:
                return LocalizationService.Ui("Awoken");
            case RelicWeaponType.AnimaAnima:
                return LocalizationService.Ui("Anima");
            case RelicWeaponType.AnimaHyperconductive:
                return LocalizationService.Ui("Hyperconductive");
            case RelicWeaponType.AnimaReconditioned:
                return LocalizationService.Ui("Reconditioned");
            case RelicWeaponType.AnimaSharpened:
                return LocalizationService.Ui("Sharpened");
            case RelicWeaponType.AnimaComplete:
                return LocalizationService.Ui("Complete");
            case RelicWeaponType.AnimaLux:
                return LocalizationService.Ui("Lux");
            case RelicWeaponType.EurekanAntiquated:
                return LocalizationService.Ui("Antiquated");
            case RelicWeaponType.EurekanBase:
                return LocalizationService.Ui("Base");
            case RelicWeaponType.EurekanBase1:
                return LocalizationService.Ui("Base + 1");
            case RelicWeaponType.EurekanBase2:
                return LocalizationService.Ui("Base + 2");
            case RelicWeaponType.EurekanAnemos:
                return LocalizationService.Ui("Anemos");
            case RelicWeaponType.EurekanPagos:
                return LocalizationService.Ui("Pagos");
            case RelicWeaponType.EurekanPagos1:
                return LocalizationService.Ui("Pagos + 1");
            case RelicWeaponType.EurekanElemental:
                return LocalizationService.Ui("Elemental");
            case RelicWeaponType.EurekanElemental1:
                return LocalizationService.Ui("Elemental + 1");
            case RelicWeaponType.EurekanElemental2:
                return LocalizationService.Ui("Elemental + 2");
            case RelicWeaponType.EurekanPyros:
                return LocalizationService.Ui("Pyros");
            case RelicWeaponType.EurekanHydatos:
                return LocalizationService.Ui("Hydatos");
            case RelicWeaponType.EurekanHydatos1:
                return LocalizationService.Ui("Hydatos + 1");
            case RelicWeaponType.EurekanBaseEureka:
                return LocalizationService.Ui("Base");
            case RelicWeaponType.EurekanEureka:
                return LocalizationService.Ui("Eureka");
            case RelicWeaponType.EurekanPhyseos:
                return LocalizationService.Ui("Physeos");
            case RelicWeaponType.ResistanceResistance:
                return LocalizationService.Ui("Resistance");
            case RelicWeaponType.ResistanceAugmentedResistance:
                return LocalizationService.Ui("Augmented Resistance");
            case RelicWeaponType.ResistanceRecollection:
                return LocalizationService.Ui("Recollection");
            case RelicWeaponType.ResistanceLawsOrder:
                return LocalizationService.Ui("Laws Order");
            case RelicWeaponType.ResistanceAugmentedLawsOrder:
                return LocalizationService.Ui("Augmented Laws Order");
            case RelicWeaponType.ResistanceBlades:
                return LocalizationService.Ui("Blades");
            case RelicWeaponType.MandervilleManderville:
                return LocalizationService.Ui("Manderville");
            case RelicWeaponType.MandervilleAmazing:
                return LocalizationService.Ui("Amazing");
            case RelicWeaponType.MandervilleMajestic:
                return LocalizationService.Ui("Majestic");
            case RelicWeaponType.MandervilleMandervillous:
                return LocalizationService.Ui("Mandervillous");
            case RelicWeaponType.PhantomPenumbrae:
                return LocalizationService.Ui("Penumbrae");
            case RelicWeaponType.PhantomUmbrae:
                return LocalizationService.Ui("Umbrae");
            case RelicWeaponType.PhantomObscurum:
                return LocalizationService.Ui("Obscurum");
        }

        return LocalizationService.Ui(relicWeaponType.ToString());
    }
}
