using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class ShopHighlightingNpcSetting : GenericBooleanSetting
{
    public ShopHighlightingNpcSetting(ILogger<ShopHighlightingNpcSetting> logger, ImGuiService imGuiService) : base("ShopHighlightingNpc", "商店高亮 - 高亮 NPC", "商店物品高亮时，是否高亮世界中的相关 NPC？", true, SettingCategory.Highlighting, SettingSubCategory.ShopHighlighting, "15.0.4", logger, imGuiService)
    {
    }
}
