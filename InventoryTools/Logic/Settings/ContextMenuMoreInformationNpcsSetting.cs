using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class ContextMenuMoreInformationNpcsSetting : GenericBooleanSetting
    {
        public ContextMenuMoreInformationNpcsSetting(ILogger<ContextMenuMoreInformationNpcsSetting> logger, ImGuiService imGuiService) : base("ContextMenuMoreInfoNpcs" , LocalizationService.Ui("Context Menu - More Information (Npcs)"), LocalizationService.Ui("Add the more information menu item to the right click/context menu for npcs?"), true, SettingCategory.ContextMenu, SettingSubCategory.General, "14.0.2", logger, imGuiService)
        {
        }
    }
}