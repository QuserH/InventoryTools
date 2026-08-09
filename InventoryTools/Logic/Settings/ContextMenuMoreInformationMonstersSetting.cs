using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class ContextMenuMoreInformationMonstersSetting : GenericBooleanSetting
    {
        public ContextMenuMoreInformationMonstersSetting(ILogger<ContextMenuMoreInformationMonstersSetting> logger, ImGuiService imGuiService) : base("ContextMenuMoreInfoMonsters" , LocalizationService.Ui("Context Menu - More Information (Monsters)"), LocalizationService.Ui("Add the more information menu item to the right click/context menu for monsters?"), true, SettingCategory.ContextMenu, SettingSubCategory.General, "14.0.2", logger, imGuiService)
        {
        }
    }
}