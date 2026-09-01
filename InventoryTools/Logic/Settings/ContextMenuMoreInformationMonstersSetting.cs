using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings
{
    public class ContextMenuMoreInformationMonstersSetting : GenericBooleanSetting
    {
        public ContextMenuMoreInformationMonstersSetting(ILogger<ContextMenuMoreInformationMonstersSetting> logger, ImGuiService imGuiService) : base("ContextMenuMoreInfoMonsters" , LocalizationService.Ui("Context Menu - More Information (Monsters)"), LocalizationService.Ui("Add the more information menu item to the right click/context menu for monsters?"), true, "14.0.2", logger, imGuiService)
        {
        }
    }
}