using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class ToolTipLocationLimitSetting : IntegerSetting
{
    public override int DefaultValue { get; set; } = 10;
    public override int CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipLocationLimit;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, int newValue)
    {
        configuration.TooltipLocationLimit = newValue;
    }

    public override string Key { get; set; } = "TooltipLocationLimit";
    public override string Name { get; set; } = "添加物品位置（最大结果数）";
    public override string HelpText { get; set; } = "悬浮提示中列出的位置数量上限。需要启用显示持有数量？。";
    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.AddItemLocations;
    public override string Version => "1.7.0.0";

    public ToolTipLocationLimitSetting(ILogger<ToolTipLocationLimitSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}