using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class HistoryEnabledSetting : BooleanSetting
{
    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.HistoryEnabled;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.HistoryEnabled = newValue;
    }

    public override string Key { get; set; } = "HistoryEnabled";
    public override string Name { get; set; } = "启用历史追踪？";
    public override string WizardName { get; } = "追踪物品历史？";

    public override string HelpText { get; set; } =
        "亚拉戈工具箱是否尝试追踪背包中物品的移动、添加和移除？";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.History;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override string Version => "1.7.0.0";

    public HistoryEnabledSetting(ILogger<HistoryEnabledSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}