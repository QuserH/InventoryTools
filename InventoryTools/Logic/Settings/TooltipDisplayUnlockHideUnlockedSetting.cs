using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayUnlockHideUnlockedSetting : GenericBooleanSetting
{
    public TooltipDisplayUnlockHideUnlockedSetting(ILogger<TooltipDisplayUnlockHideUnlockedSetting> logger, ImGuiService imGuiService) : base("TooltipDisplayUnlockHideUnlocked", "添加物品解锁状态（隐藏已解锁角色）", "已解锁的角色是否应隐藏？分组模式下将隐藏已获得组。", false, SettingCategory.ToolTips, SettingSubCategory.ItemUnlockStatus, "1.11.1.1", logger, imGuiService)
    {
    }
}