using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class CompendiumRowHeightSetting : GenericIntegerSetting
{
    public CompendiumRowHeightSetting(ILogger<CompendiumRowHeightSetting> logger, ImGuiService imGuiService) : base("CompendiumRowHeight", LocalizationService.Ui("Row Height"), LocalizationService.Ui("What should the minimum height of rows show in compendium lists be?"), 32, "14.0.3", logger, imGuiService)
    {
    }
}