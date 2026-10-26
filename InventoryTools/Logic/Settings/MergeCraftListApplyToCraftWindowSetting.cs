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
    public override string Name { get; set; } = "制作列表生效";
    public override string HelpText { get; set; } =
        "是否在制作列表的“雇员/背包中的物品”面板中应用合并？";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.General;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override uint? Order => 2;
    public override string? ParentKey => "MergeCraftListSameSource";
    public override string Version => "1.7.0.0";
}
