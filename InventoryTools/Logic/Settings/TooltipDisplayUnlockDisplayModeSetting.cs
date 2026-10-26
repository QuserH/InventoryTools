using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public enum TooltipDisplayUnlockDisplayMode
{
    CharacterPerLine,
    GroupedByUnlockStatus
}

public class TooltipDisplayUnlockDisplayModeSetting : GenericEnumChoiceSetting<TooltipDisplayUnlockDisplayMode>
{
    public TooltipDisplayUnlockDisplayModeSetting(ILogger<TooltipDisplayUnlockDisplayModeSetting> logger, ImGuiService imGuiService) : base("TooltipDisplayUnlockDisplayMode", "添加物品解锁状态（显示模式）", "物品解锁状态悬浮提示应如何显示？", TooltipDisplayUnlockDisplayMode.CharacterPerLine, new Dictionary<TooltipDisplayUnlockDisplayMode, string>()
    {
        { TooltipDisplayUnlockDisplayMode.CharacterPerLine , "每行一个角色" },
        { TooltipDisplayUnlockDisplayMode.GroupedByUnlockStatus , "按解锁状态分组" },
    }, SettingCategory.ToolTips, SettingSubCategory.ItemUnlockStatus, "1.11.1.1", logger, imGuiService)
    {
    }
}