using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipFooterLinesSetting : IntegerSetting
{
    public override int DefaultValue { get; set; } = 0;
    public override int CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipFooterLines;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, int newValue)
    {
        configuration.TooltipFooterLines = newValue;
    }

    public override string Key { get; set; } = "TooltipFooterLines";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Footer New Lines"));

    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("How many new lines should be added below any tooltip modifications made by this plugin?"));

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.Visuals;
    public override string Version => "1.7.0.0";

    public TooltipFooterLinesSetting(ILogger<TooltipFooterLinesSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}