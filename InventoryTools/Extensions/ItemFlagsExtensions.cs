using FFXIVClientStructs.FFXIV.Client.Game;
using InventoryTools.Localization;

namespace InventoryTools.Extensions;

public static class ItemFlagsExtensions
{
    public static string FormattedName(this InventoryItem.ItemFlags flags)
    {
        return flags switch
        {
            InventoryItem.ItemFlags.None => LocalizationService.Ui("Normal Quality"),
            InventoryItem.ItemFlags.HighQuality => LocalizationService.Ui("High Quality"),
            InventoryItem.ItemFlags.CompanyCrestApplied => LocalizationService.Ui("Company Crest Applied"),
            InventoryItem.ItemFlags.Relic => LocalizationService.Ui("Relic"),
            InventoryItem.ItemFlags.Collectable => LocalizationService.Ui("Collectable"),
            _ => LocalizationService.Ui("None")
        };
    }
}