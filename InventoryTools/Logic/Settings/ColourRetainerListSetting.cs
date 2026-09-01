using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class ColourRetainerListSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = true;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.ColorRetainerList;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.ColorRetainerList = newValue;
        }

        public override string Key { get; set; } = "ColourRetainerList";
        public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Color name in retainer list?"));
        public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Should the name of the retainer in the summoning bell list be coloured if a relevant item is to be sorted or is available in their inventory?"));
        public override string Version => "1.7.0.0";

        public ColourRetainerListSetting(ILogger<ColourRetainerListSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}