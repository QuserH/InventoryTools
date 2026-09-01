using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class MergeCraftListApplyToItemWindowSetting : BooleanSetting
{
    public MergeCraftListApplyToItemWindowSetting(ILogger<MergeCraftListApplyToItemWindowSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = true;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.MergeCraftListApplyToItemWindow;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.MergeCraftListApplyToItemWindow = newValue;
    }

    public override string Key { get; set; } = "MergeCraftListApplyToItemWindow";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Apply merge to item window?"));
    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("Should the merge apply to the item window opened by /atools?"));

    public override string Version => "1.7.0.0";
}
