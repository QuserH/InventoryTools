using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class ShopHighlightingNpcNameplateIconSetting : GenericBooleanSetting
{
    public ShopHighlightingNpcNameplateIconSetting(ILogger<ShopHighlightingNpcNameplateIconSetting> logger, ImGuiService imGuiService)
        : base(
            "ShopHighlightingNpcNameplateIcon",
            "商店高亮 - 名牌图标",
            "商店物品高亮时，在世界中相关 NPC 的名牌上显示小商人图标。",
            true,
            SettingCategory.Highlighting,
            SettingSubCategory.ShopHighlighting,
            "15.0.4",
            logger,
            imGuiService)
    {
    }
}
