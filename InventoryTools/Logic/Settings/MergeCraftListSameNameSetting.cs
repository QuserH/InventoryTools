using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class MergeCraftListSameNameSetting : BooleanSetting
{
    public MergeCraftListSameNameSetting(ILogger<MergeCraftListSameNameSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.MergeCraftListSameName;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.MergeCraftListSameName = newValue;
    }

    public override string Key { get; set; } = "MergeCraftListSameName";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Merge same name items from all sources in craft list?"));
    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("Should items with the same name be merged into a single row regardless of which character or retainer holds them, including HQ and NQ variants?"));

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.General;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override string Version => "1.7.0.0";
}
