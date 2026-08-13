using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class MergeCraftListSameSourceSetting : BooleanSetting
{
    public MergeCraftListSameSourceSetting(ILogger<MergeCraftListSameSourceSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = true;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.MergeCraftListSameSource;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.MergeCraftListSameSource = newValue;
    }

    public override string Key { get; set; } = "MergeCraftListSameSource";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Merge same source items in craft list?"));
    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("Should stacks of the same item held by the same character and retainer be merged into a single row in the craft list inventory panel?"));

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.General;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override string Version => "1.7.0.0";
}
