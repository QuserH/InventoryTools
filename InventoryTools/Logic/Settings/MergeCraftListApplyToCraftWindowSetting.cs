using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class MergeCraftListApplyToCraftWindowSetting : BooleanSetting
{
    public MergeCraftListApplyToCraftWindowSetting(ILogger<MergeCraftListApplyToCraftWindowSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = true;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.MergeCraftListApplyToCraftWindow;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.MergeCraftListApplyToCraftWindow = newValue;
    }

    public override string Key { get; set; } = "MergeCraftListApplyToCraftWindow";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Apply merge to craft list?"));
    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("Should the merge apply to the craft list inventory panel?"));

    public override string Version => "1.7.0.0";
}
