using InventoryTools.Compendium.Types.Extra;
using InventoryTools.Localization;

namespace InventoryTools.Localizers;

public class ChocoboItemSourceTypeLocalizer : ILocalizer<ChocoboItemSourceType>
{
    public string Format(ChocoboItemSourceType itemSourceType)
    {
        switch (itemSourceType)
        {
            case ChocoboItemSourceType.BuddyItem:
                return "消耗品";
            case ChocoboItemSourceType.BuddyEquip:
                return "装备";
        }

        return "未知";
    }
}
