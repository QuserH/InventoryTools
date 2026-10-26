using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class CraftOverlayMaxExpandedItemsSetting : GenericIntegerSetting
{
    public CraftOverlayMaxExpandedItemsSetting(ILogger<CraftOverlayMaxExpandedItemsSetting> logger,
        ImGuiService imGuiService) : base("CraftOverlayMaxItems",
        "展开时的最大物品数",
        "制作悬浮窗展开时应显示多少件物品？",
        5,
        SettingCategory.CraftOverlay,
        SettingSubCategory.General,
        "1.11.0.8",
        logger,
        imGuiService)
    {
    }
}