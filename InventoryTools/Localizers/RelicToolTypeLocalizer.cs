using System;
using LuminaSupplemental.Excel.Model;
using InventoryTools.Localization;

namespace InventoryTools.Localizers;

public class RelicToolTypeLocalizer : ILocalizer<RelicToolType>
{
    public string Format(RelicToolType relicToolType)
    {
        switch (relicToolType)
        {
            case RelicToolType.Unknown:
                return LocalizationService.Ui("Unknown");
            case RelicToolType.MastercraftBase:
                return LocalizationService.Ui("Relic Mastercraft Base");
            case RelicToolType.MastercraftSupra:
                return LocalizationService.Ui("Relic Mastercraft Supra");
            case RelicToolType.MastercraftLucis:
                return LocalizationService.Ui("Relic Mastercraft Lucis");
            case RelicToolType.SkysteelBase:
                return LocalizationService.Ui("Relic Skysteel Base");
            case RelicToolType.SkysteelBase1:
                return LocalizationService.Ui("Relic Skysteel Base + 1");
            case RelicToolType.SkysteelDragonsung:
                return LocalizationService.Ui("Relic Skysteel Dragonsung");
            case RelicToolType.SkysteelAugmentedDragonsung:
                return LocalizationService.Ui("Relic Skysteel Augmented Dragonsung");
            case RelicToolType.SkysteelSkysung:
                return LocalizationService.Ui("Relic Skysteel Skysung");
            case RelicToolType.SkysteelSkybuilders:
                return LocalizationService.Ui("Relic Skysteel Skybuilders");
            case RelicToolType.Resplendent:
                return LocalizationService.Ui("Relic Resplendent");
            case RelicToolType.SplendorousBase:
                return LocalizationService.Ui("Relic Splendorous Base");
            case RelicToolType.SplendorousAugmented:
                return LocalizationService.Ui("Relic Splendorous Augmented");
            case RelicToolType.SplendorousCrystalline:
                return LocalizationService.Ui("Relic Splendorous Crystalline");
            case RelicToolType.SplendorousChoraZoiCrystalline:
                return LocalizationService.Ui("Relic Splendorous Chora-Zoi Crystalline");
            case RelicToolType.SplendorousBrilliant:
                return LocalizationService.Ui("Relic Splendorous Brilliant");
            case RelicToolType.SplendorousVrandticVisionary:
                return LocalizationService.Ui("Relic Splendorous Vrandtic Visionary");
            case RelicToolType.SplendorousLodestar:
                return LocalizationService.Ui("Relic Splendorous Lodestar");
            case RelicToolType.CosmicPrototype01:
                return LocalizationService.Ui("Relic Cosmic Prototype01");
            case RelicToolType.CosmicPrototype02:
                return LocalizationService.Ui("Relic Cosmic Prototype02");
            case RelicToolType.CosmicPrototype03:
                return LocalizationService.Ui("Relic Cosmic Prototype03");
            case RelicToolType.CosmicPrototype04:
                return LocalizationService.Ui("Relic Cosmic Prototype04");
            case RelicToolType.CosmicPrototype05:
                return LocalizationService.Ui("Relic Cosmic Prototype05");
            case RelicToolType.CosmicPrototype06:
                return LocalizationService.Ui("Relic Cosmic Prototype06");
            case RelicToolType.CosmicPrototype07:
                return LocalizationService.Ui("Relic Cosmic Prototype07");
            case RelicToolType.CosmicPrototype08:
                return LocalizationService.Ui("Relic Cosmic Prototype08");
            case RelicToolType.CosmicCosmic:
                return LocalizationService.Ui("Relic Cosmic Cosmic");
            case RelicToolType.CosmicCosmic11:
                return LocalizationService.Ui("Relic Cosmic Cosmic 1.1");
            case RelicToolType.CosmicCosmic12:
                return LocalizationService.Ui("Relic Cosmic Cosmic 1.2");
            case RelicToolType.CosmicCosmic13:
                return LocalizationService.Ui("Relic Cosmic Cosmic 1.3");
            case RelicToolType.CosmicCosmic14:
                return LocalizationService.Ui("Relic Cosmic Cosmic 1.4");
            case RelicToolType.CosmicStellar:
                return LocalizationService.Ui("Relic Cosmic Stellar");
            case RelicToolType.CosmicStellar11:
                return LocalizationService.Ui("Relic Cosmic Stellar 1.1");
            case RelicToolType.CosmicStellar12:
                return LocalizationService.Ui("Relic Cosmic Stellar 1.2");
            case RelicToolType.CosmicHypertools:
                return LocalizationService.Ui("Relic Cosmic Hypertools");
        }

        return LocalizationService.Ui(relicToolType.ToString());
    }
}
