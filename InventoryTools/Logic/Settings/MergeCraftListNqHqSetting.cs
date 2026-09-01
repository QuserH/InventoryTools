using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class MergeCraftListNqHqSetting : BooleanSetting
{
    public MergeCraftListNqHqSetting(ILogger<MergeCraftListNqHqSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.MergeCraftListNqHq;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.MergeCraftListNqHq = newValue;
    }

    public override string Key { get; set; } = "MergeCraftListNqHq";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Merge HQ and NQ in craft list?"));
    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("When the same character, source and item name have both NQ and HQ stacks, should they be merged into one row without showing quality? When disabled, different qualities are kept separate."));

    public override string Version => "1.7.0.0";
}
