using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class CraftOverlayRememberStateSetting : GenericBooleanSetting
{
    public CraftOverlayRememberStateSetting(ILogger<CraftOverlayRememberStateSetting> logger,
        ImGuiService imGuiService) : base("CraftOverlayRememberState",
        "记住状态",
        "制作悬浮窗是否应在插件重载/游戏重载之间保持打开？",
        true,
        SettingCategory.CraftOverlay,
        SettingSubCategory.General,
        "1.11.0.8",
        logger,
        imGuiService)
    {
    }
}