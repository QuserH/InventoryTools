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
                return LocalizationService.Ui("Consumable");
            case ChocoboItemSourceType.BuddyEquip:
                return LocalizationService.Ui("Equipment");
        }

        return LocalizationService.Ui("Unknown");
    }
}
