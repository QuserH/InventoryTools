using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class CraftOverlayMaxExpandedItemsSetting : GenericIntegerSetting
{
    public CraftOverlayMaxExpandedItemsSetting(ILogger<CraftOverlayMaxExpandedItemsSetting> logger,
        ImGuiService imGuiService) : base("CraftOverlayMaxItems",
        LocalizationService.Ui("Max items when expanded"),
        LocalizationService.Ui("When the craft overlay is expanded, how many items should be shown?"),
        5,
        "1.11.0.8",
        logger,
        imGuiService)
    {
    }
}