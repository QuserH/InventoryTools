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
                return LocalizationService.Ui("Relic Zodiac Base");
            case RelicWeaponType.ZodiacZenith:
                return LocalizationService.Ui("Relic Zodiac Zenith");
            case RelicWeaponType.ZodiacAtma:
                return LocalizationService.Ui("Relic Zodiac Atma");
            case RelicWeaponType.ZodiacAnimus:
                return LocalizationService.Ui("Relic Zodiac Animus");
            case RelicWeaponType.ZodiacNovus:
                return LocalizationService.Ui("Relic Zodiac Novus");
            case RelicWeaponType.ZodiacNexus:
                return LocalizationService.Ui("Relic Zodiac Nexus");
            case RelicWeaponType.ZodiacZodiac:
                return LocalizationService.Ui("Relic Zodiac Zodiac");
            case RelicWeaponType.ZodiacZeta:
                return LocalizationService.Ui("Relic Zodiac Zeta");
            case RelicWeaponType.AnimaAnimated:
                return LocalizationService.Ui("Relic Anima Animated");
            case RelicWeaponType.AnimaAwoken:
                return LocalizationService.Ui("Relic Anima Awoken");
            case RelicWeaponType.AnimaAnima:
                return LocalizationService.Ui("Relic Anima Anima");
            case RelicWeaponType.AnimaHyperconductive:
                return LocalizationService.Ui("Relic Anima Hyperconductive");
            case RelicWeaponType.AnimaReconditioned:
                return LocalizationService.Ui("Relic Anima Reconditioned");
            case RelicWeaponType.AnimaSharpened:
                return LocalizationService.Ui("Relic Anima Sharpened");
            case RelicWeaponType.AnimaComplete:
                return LocalizationService.Ui("Relic Anima Complete");
            case RelicWeaponType.AnimaLux:
                return LocalizationService.Ui("Relic Anima Lux");
            case RelicWeaponType.EurekanAntiquated:
                return LocalizationService.Ui("Relic Eurekan Antiquated");
            case RelicWeaponType.EurekanBase:
                return LocalizationService.Ui("Relic Eurekan Base");
            case RelicWeaponType.EurekanBase1:
                return LocalizationService.Ui("Relic Eurekan Base + 1");
            case RelicWeaponType.EurekanBase2:
                return LocalizationService.Ui("Relic Eurekan Base + 2");
            case RelicWeaponType.EurekanAnemos:
                return LocalizationService.Ui("Relic Eurekan Anemos");
            case RelicWeaponType.EurekanPagos:
                return LocalizationService.Ui("Relic Eurekan Pagos");
            case RelicWeaponType.EurekanPagos1:
                return LocalizationService.Ui("Relic Eurekan Pagos + 1");
            case RelicWeaponType.EurekanElemental:
                return LocalizationService.Ui("Relic Eurekan Elemental");
            case RelicWeaponType.EurekanElemental1:
                return LocalizationService.Ui("Relic Eurekan Elemental + 1");
            case RelicWeaponType.EurekanElemental2:
                return LocalizationService.Ui("Relic Eurekan Elemental + 2");
            case RelicWeaponType.EurekanPyros:
                return LocalizationService.Ui("Relic Eurekan Pyros");
            case RelicWeaponType.EurekanHydatos:
                return LocalizationService.Ui("Relic Eurekan Hydatos");
            case RelicWeaponType.EurekanHydatos1:
                return LocalizationService.Ui("Relic Eurekan Hydatos + 1");
            case RelicWeaponType.EurekanBaseEureka:
                return LocalizationService.Ui("Relic Eurekan Base Eureka");
            case RelicWeaponType.EurekanEureka:
                return LocalizationService.Ui("Relic Eurekan Eureka");
            case RelicWeaponType.EurekanPhyseos:
                return LocalizationService.Ui("Relic Eurekan Physeos");
            case RelicWeaponType.ResistanceResistance:
                return LocalizationService.Ui("Relic Resistance Resistance");
            case RelicWeaponType.ResistanceAugmentedResistance:
                return LocalizationService.Ui("Relic Resistance Augmented Resistance");
            case RelicWeaponType.ResistanceRecollection:
                return LocalizationService.Ui("Relic Resistance Recollection");
            case RelicWeaponType.ResistanceLawsOrder:
                return LocalizationService.Ui("Relic Resistance Laws Order");
            case RelicWeaponType.ResistanceAugmentedLawsOrder:
                return LocalizationService.Ui("Relic Resistance Augmented Laws Order");
            case RelicWeaponType.ResistanceBlades:
                return LocalizationService.Ui("Relic Resistance Blades");
            case RelicWeaponType.MandervilleManderville:
                return LocalizationService.Ui("Relic Manderville Manderville");
            case RelicWeaponType.MandervilleAmazing:
                return LocalizationService.Ui("Relic Manderville Amazing");
            case RelicWeaponType.MandervilleMajestic:
                return LocalizationService.Ui("Relic Manderville Majestic");
            case RelicWeaponType.MandervilleMandervillous:
                return LocalizationService.Ui("Relic Manderville Mandervillous");
            case RelicWeaponType.PhantomPenumbrae:
                return LocalizationService.Ui("Relic Phantom Penumbrae");
            case RelicWeaponType.PhantomUmbrae:
                return LocalizationService.Ui("Relic Phantom Umbrae");
            case RelicWeaponType.PhantomObscurum:
                return LocalizationService.Ui("Relic Phantom Obscurum");
        }

        return LocalizationService.Ui(relicWeaponType.ToString());
    }
}
