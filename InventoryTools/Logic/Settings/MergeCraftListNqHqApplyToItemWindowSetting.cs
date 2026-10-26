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
    public override string Name { get; set; } = "物品窗口生效";
    public override string HelpText { get; set; } =
        "是否在物品窗口中合并NQ和HQ？";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.General;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override uint? Order => 6;
    public override string? ParentKey => "MergeCraftListNqHq";
    public override string Version => "1.7.0.0";
}
