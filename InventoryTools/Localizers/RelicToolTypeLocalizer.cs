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
                return "Unknown";
            case RelicToolType.MastercraftBase:
                return "Mastercraft";
            case RelicToolType.MastercraftSupra:
                return "Supra";
            case RelicToolType.MastercraftLucis:
                return "Lucis";
            case RelicToolType.SkysteelBase:
                return "Skysteel";
            case RelicToolType.SkysteelBase1:
                return LocalizationService.Ui("Skysteel + 1");
            case RelicToolType.SkysteelDragonsung:
                return "Dragonsung";
            case RelicToolType.SkysteelAugmentedDragonsung:
                return LocalizationService.Ui("Augmented Dragonsung");
            case RelicToolType.SkysteelSkysung:
                return "Skysung";
            case RelicToolType.SkysteelSkybuilders:
                return "Skybuilders";
            case RelicToolType.Resplendent:
                return "Resplendent";
            case RelicToolType.SplendorousBase:
                return "Spendorous";
            case RelicToolType.SplendorousAugmented:
                return LocalizationService.Ui("Augmented Splendorous");
            case RelicToolType.SplendorousCrystalline:
                return "Crystalline";
            case RelicToolType.SplendorousChoraZoiCrystalline:
                return LocalizationService.Ui("Chora-Zoi's Crystalline");
            case RelicToolType.SplendorousBrilliant:
                return "Brilliant";
            case RelicToolType.SplendorousVrandticVisionary:
                return LocalizationService.Ui("Vrandtic Visionary's");
            case RelicToolType.SplendorousLodestar:
                return "Lodestar";
            case RelicToolType.CosmicPrototype01:
                return LocalizationService.Ui("Prototype v0.1");
            case RelicToolType.CosmicPrototype02:
                return LocalizationService.Ui("Prototype v0.2");
            case RelicToolType.CosmicPrototype03:
                return LocalizationService.Ui("Prototype v0.3");
            case RelicToolType.CosmicPrototype04:
                return LocalizationService.Ui("Prototype v0.4");
            case RelicToolType.CosmicPrototype05:
                return LocalizationService.Ui("Prototype v0.5");
            case RelicToolType.CosmicPrototype06:
                return LocalizationService.Ui("Prototype v0.6");
            case RelicToolType.CosmicPrototype07:
                return LocalizationService.Ui("Prototype v0.7");
            case RelicToolType.CosmicPrototype08:
                return LocalizationService.Ui("Prototype v0.8");
            case RelicToolType.CosmicCosmic:
                return "Cosmic";
            case RelicToolType.CosmicCosmic11:
                return LocalizationService.Ui("Cosmic v1.1");
            case RelicToolType.CosmicCosmic12:
                return LocalizationService.Ui("Cosmic v1.2");
            case RelicToolType.CosmicCosmic13:
                return LocalizationService.Ui("Cosmic v1.3");
            case RelicToolType.CosmicCosmic14:
                return LocalizationService.Ui("Cosmic v1.4");
            case RelicToolType.CosmicStellar:
                return "Stellar";
            case RelicToolType.CosmicStellar11:
                return LocalizationService.Ui("Stellar v1.1");
            case RelicToolType.CosmicStellar12:
                return LocalizationService.Ui("Stellar v1.2");
            case RelicToolType.CosmicHypertools:
                return "Hypertools";
        }

        return relicToolType.ToString();
    }
}