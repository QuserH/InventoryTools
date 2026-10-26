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
                return "未知";
            case RelicToolType.MastercraftBase:
                return "华光工具";
            case RelicToolType.MastercraftSupra:
                return "赐福工具";
            case RelicToolType.MastercraftLucis:
                return "辉煌工具";
            case RelicToolType.SkysteelBase:
                return "天钢工具";
            case RelicToolType.SkysteelBase1:
                return "天钢工具+1";
            case RelicToolType.SkysteelDragonsung:
                return "龙诗工具";
            case RelicToolType.SkysteelAugmentedDragonsung:
                return "改良型龙诗工具";
            case RelicToolType.SkysteelSkysung:
                return "天诗工具";
            case RelicToolType.SkysteelSkybuilders:
                return "天工工具";
            case RelicToolType.Resplendent:
                return "天穹工具";
            case RelicToolType.SplendorousBase:
                return "卓越工具";
            case RelicToolType.SplendorousAugmented:
                return "改良型卓越工具";
            case RelicToolType.SplendorousCrystalline:
                return "水晶工具";
            case RelicToolType.SplendorousChoraZoiCrystalline:
                return "乔菈水晶工具";
            case RelicToolType.SplendorousBrilliant:
                return "乔菈卓绝工具";
            case RelicToolType.SplendorousVrandticVisionary:
                return "诺弗兰特远见工具";
            case RelicToolType.SplendorousLodestar:
                return "领航星工具";
            case RelicToolType.CosmicPrototype01:
                return "宇宙工具·α";
            case RelicToolType.CosmicPrototype02:
                return "宇宙工具·β";
            case RelicToolType.CosmicPrototype03:
                return "宇宙工具·γ";
            case RelicToolType.CosmicPrototype04:
                return "宇宙工具·δ";
            case RelicToolType.CosmicPrototype05:
                return "宇宙工具·ε";
            case RelicToolType.CosmicPrototype06:
                return "宇宙工具·ζ";
            case RelicToolType.CosmicPrototype07:
                return "宇宙工具·η";
            case RelicToolType.CosmicPrototype08:
                return "宇宙工具·θ";
            case RelicToolType.CosmicCosmic:
                return "宇宙工具";
            case RelicToolType.CosmicCosmic11:
                return "太空工具·α";
            case RelicToolType.CosmicCosmic12:
                return "太空工具·β";
            case RelicToolType.CosmicCosmic13:
                return "太空工具·γ";
            case RelicToolType.CosmicCosmic14:
                return "太空工具·δ";
            case RelicToolType.CosmicStellar:
                return "超空间工具";
            case RelicToolType.CosmicStellar11:
                return "超空间工具·α";
            case RelicToolType.CosmicStellar12:
                return "超空间工具·β";
            case RelicToolType.CosmicHypertools:
                return "群星工具";
        }

        return LocalizationService.Ui(relicToolType.ToString());
    }
}
