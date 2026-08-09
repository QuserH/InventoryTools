using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using InventoryTools.Ui;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class CraftOverlayWindowStateSetting : GenericEnumChoiceSetting<CraftOverlayWindowState>
{
    public CraftOverlayWindowStateSetting(ILogger<CraftOverlayWindowStateSetting> logger, ImGuiService imGuiService) : base("CraftOverlayWindowState", LocalizationService.Ui("Window State"), LocalizationService.Ui("The current state of the craft overlay window."), CraftOverlayWindowState.Single, new Dictionary<CraftOverlayWindowState, string>()
    {
        { CraftOverlayWindowState.Collapsed, "Collapsed"},
        { CraftOverlayWindowState.Single, "Single"},
        { CraftOverlayWindowState.List, "Expanded"},
    }, SettingCategory.None, SettingSubCategory.None, "1.11.0.8", logger, imGuiService)
    {
    }
}