using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class ContextMenuMoreInformationNpcsSetting : GenericBooleanSetting
    {
        public ContextMenuMoreInformationNpcsSetting(ILogger<ContextMenuMoreInformationNpcsSetting> logger, ImGuiService imGuiService) : base("ContextMenuMoreInfoNpcs" , "快捷菜单 - 更多信息（NPC）", "是否在 NPC 的右键/快捷菜单中添加更多信息菜单项？", true, SettingCategory.ContextMenu, SettingSubCategory.General, "14.0.2", logger, imGuiService)
        {
        }
    }
}