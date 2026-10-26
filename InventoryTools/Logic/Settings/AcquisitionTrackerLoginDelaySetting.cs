using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class AcquisitionTrackerLoginDelaySetting : GenericIntegerSetting
{
    public AcquisitionTrackerLoginDelaySetting(ILogger<AcquisitionTrackerLoginDelaySetting> logger, ImGuiService imGuiService) : base("AcquisitionTrackerLoginDelay", "登录延迟", "获取追踪器会在你登录时对角色的背包执行初始扫描。由于游戏会收到多次背包更新，扫描器必须等待设定时间才能完成。若首次登录时发现制作列表中的物品随机完成，可调高此值。", 5, SettingCategory.Troubleshooting, SettingSubCategory.AcquisitionTracker,"1.12.0.8", logger, imGuiService)
    {
    }
}