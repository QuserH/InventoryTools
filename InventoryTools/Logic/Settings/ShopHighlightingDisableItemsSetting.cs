using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class ShopHighlightingDisableItemsSetting : GenericBooleanSetting
{
    public ShopHighlightingDisableItemsSetting(ILogger<ShopHighlightingDisableItemsSetting> logger, ImGuiService imGuiService) : base("ShopHighlightingDisableItems", "商店高亮 - 禁用物品", "商店物品高亮时，是否禁用未高亮的物品？", false, SettingCategory.Highlighting, SettingSubCategory.ShopHighlighting, "15.0.4", logger, imGuiService)
    {
    }
}