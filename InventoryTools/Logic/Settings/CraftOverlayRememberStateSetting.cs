using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class CraftOverlayRememberStateSetting : GenericBooleanSetting
{
    public CraftOverlayRememberStateSetting(ILogger<CraftOverlayRememberStateSetting> logger,
        ImGuiService imGuiService) : base("CraftOverlayRememberState",
        LocalizationService.Ui("Remember State"),
        LocalizationService.Ui("Should the craft overlay stay open between plugin reloads/game reloads?"),
        true,
        "1.11.0.8",
        logger,
        imGuiService)
    {
    }
}