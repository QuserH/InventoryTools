using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class MergeCraftListNqHqApplyToItemWindowSetting : BooleanSetting
{
    public MergeCraftListNqHqApplyToItemWindowSetting(ILogger<MergeCraftListNqHqApplyToItemWindowSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = true;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.MergeCraftListNqHqApplyToItemWindow;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.MergeCraftListNqHqApplyToItemWindow = newValue;
    }

    public override string Key { get; set; } = "MergeCraftListNqHqApplyToItemWindow";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Apply NQ/HQ merge to item window?"));
    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("Should the NQ/HQ merge apply to the item window opened by /atools?"));

    public override string Version => "1.7.0.0";
}
