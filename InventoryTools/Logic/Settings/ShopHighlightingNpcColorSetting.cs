using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class ShopHighlightingNpcColorSetting : GenericEnumChoiceSetting<ObjectHighlightColor>
{
    public ShopHighlightingNpcColorSetting(ILogger<ShopHighlightingNpcColorSetting> logger, ImGuiService imGuiService) : base("ShopHighlightingNpcColor", "商店高亮 - NPC 高亮颜色", "商店物品高亮时，用于高亮世界中 NPC 的颜色。", ObjectHighlightColor.Green, new Dictionary<ObjectHighlightColor, string>
    {
        { ObjectHighlightColor.Red,     "Red"     },
        { ObjectHighlightColor.Green,   "Green"   },
        { ObjectHighlightColor.Blue,    "Blue"    },
        { ObjectHighlightColor.Yellow,  "Yellow"  },
        { ObjectHighlightColor.Orange,  "Orange"  },
        { ObjectHighlightColor.Magenta, "Magenta" },
    }, SettingCategory.Highlighting, SettingSubCategory.ShopHighlighting, "15.0.4", logger, imGuiService)
    {
    }
}
