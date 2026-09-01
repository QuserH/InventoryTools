using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class MergeCraftListNqHqApplyToCraftWindowSetting : BooleanSetting
{
    public MergeCraftListNqHqApplyToCraftWindowSetting(ILogger<MergeCraftListNqHqApplyToCraftWindowSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = true;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.MergeCraftListNqHqApplyToCraftWindow;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.MergeCraftListNqHqApplyToCraftWindow = newValue;
    }

    public override string Key { get; set; } = "MergeCraftListNqHqApplyToCraftWindow";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Apply NQ/HQ merge to craft list?"));
    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("Should the NQ/HQ merge apply to the craft list inventory panel?"));

    public override string Version => "1.7.0.0";
}
