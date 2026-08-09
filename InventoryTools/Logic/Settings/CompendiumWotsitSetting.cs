using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class CompendiumWotsitSetting : GenericBooleanSetting
{
    public CompendiumWotsitSetting(ILogger<CompendiumWotsitSetting> logger, ImGuiService imGuiService) : base("CompendiumWotsitSetting", LocalizationService.Ui("Enable Wotsit Integration?"), LocalizationService.Ui("Should the compendium windows be listed in wotsit's search?"), true, SettingCategory.Compendium, SettingSubCategory.General, "14.1.0", logger, imGuiService)
    {
    }
}