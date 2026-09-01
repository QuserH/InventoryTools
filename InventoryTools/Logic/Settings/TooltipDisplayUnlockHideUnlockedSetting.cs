using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayUnlockHideUnlockedSetting : GenericBooleanSetting
{
    public TooltipDisplayUnlockHideUnlockedSetting(ILogger<TooltipDisplayUnlockHideUnlockedSetting> logger, ImGuiService imGuiService) : base("TooltipDisplayUnlockHideUnlocked", LocalizationService.Ui("Add Item Unlock Status (Hide Unlocked Characters)"), LocalizationService.Ui("Should characters that already have this unlocked be hidden? If in grouped mode, this will hide the acquired group."), false, "1.11.1.1", logger, imGuiService)
    {
    }
}