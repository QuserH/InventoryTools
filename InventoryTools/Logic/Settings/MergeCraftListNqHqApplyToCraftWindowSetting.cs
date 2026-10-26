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
    public override string Name { get; set; } = "制作列表生效";
    public override string HelpText { get; set; } =
        "是否在制作列表的“雇员/背包中的物品”面板中合并NQ和HQ？";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.General;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override uint? Order => 5;
    public override string? ParentKey => "MergeCraftListNqHq";
    public override string Version => "1.7.0.0";
}
