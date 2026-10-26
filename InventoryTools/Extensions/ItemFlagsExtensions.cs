using FFXIVClientStructs.FFXIV.Client.Game;
using InventoryTools.Localization;

namespace InventoryTools.Extensions;

public static class ItemFlagsExtensions
{
    public static string FormattedName(this InventoryItem.ItemFlags flags)
    {
        return flags switch
        {
            InventoryItem.ItemFlags.None => "Normal Quality",
            InventoryItem.ItemFlags.HighQuality => "High Quality",
            InventoryItem.ItemFlags.CompanyCrestApplied => "已应用部队徽章",
            InventoryItem.ItemFlags.Relic => "发光武器",
            InventoryItem.ItemFlags.Collectable => "收藏品",
            _ => "无"
        };
    }
}