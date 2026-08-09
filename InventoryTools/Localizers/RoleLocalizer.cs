using AllaganLib.GameSheets.Sheets.Rows;
using InventoryTools.Localization;

namespace InventoryTools.Localizers;

public class RoleLocalizer : ILocalizer<RoleType>
{
    public string Format(RoleType instance)
    {
        switch (instance)
        {
            case RoleType.Tank:
                return LocalizationService.Ui("Tank");
            case RoleType.DPSMelee:
                return LocalizationService.Ui("DPS (Melee)");
            case RoleType.DPSRanged:
                return LocalizationService.Ui("DPS (Ranged)");
            case RoleType.Healer:
                return LocalizationService.Ui("Healer");
            case RoleType.Crafting:
                return LocalizationService.Ui("Crafting");
            case RoleType.Gathering:
                return LocalizationService.Ui("Gathering");
            case RoleType.Other:
                return LocalizationService.Ui("Other");
        }

        return LocalizationService.Ui("Unknown");
    }
}