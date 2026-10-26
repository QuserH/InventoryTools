using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class MergeCraftListNqHqSetting : BooleanSetting
{
    public MergeCraftListNqHqSetting(ILogger<MergeCraftListNqHqSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.MergeCraftListNqHq;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.MergeCraftListNqHq = newValue;
    }

    public override string Key { get; set; } = "MergeCraftListNqHq";
    public override string Name { get; set; } = "是否合并NQ和HQ？";
    public override string HelpText { get; set; } =
        "同一角色、来源、道具名称下同时存在NQ和HQ时，是否合并为一行且不显示品质？关闭时不同品质分开显示。";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.General;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override uint? Order => 4;
    public override string? ParentKey => "MergeCraftListSameSource";
    public override string Version => "1.7.0.0";
}
