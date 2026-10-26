using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class ContextMenuMoreInformationMonstersSetting : GenericBooleanSetting
    {
        public ContextMenuMoreInformationMonstersSetting(ILogger<ContextMenuMoreInformationMonstersSetting> logger, ImGuiService imGuiService) : base("ContextMenuMoreInfoMonsters" , "快捷菜单 - 更多信息（怪物）", "是否在怪物的右键/快捷菜单中添加更多信息菜单项？", true, SettingCategory.ContextMenu, SettingSubCategory.General, "14.0.2", logger, imGuiService)
        {
        }
    }
}