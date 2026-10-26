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
                return "坦克";
            case RoleType.DPSMelee:
                return "DPS（近战）";
            case RoleType.DPSRanged:
                return "DPS（远程）";
            case RoleType.Healer:
                return "治疗";
            case RoleType.Crafting:
                return "制作";
            case RoleType.Gathering:
                return "采集";
            case RoleType.Other:
                return "其他";
        }

        return "未知";
    }
}