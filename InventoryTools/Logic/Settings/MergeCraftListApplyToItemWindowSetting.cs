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
    public override string Name { get; set; } = "物品窗口生效";
    public override string HelpText { get; set; } =
        "是否在物品窗口中应用合并？";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.General;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override uint? Order => 3;
    public override string? ParentKey => "MergeCraftListSameSource";
    public override string Version => "1.7.0.0";
}
