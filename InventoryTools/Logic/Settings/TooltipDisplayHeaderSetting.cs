using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayHeaderSetting : BooleanSetting
{
    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipDisplayHeader;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.TooltipDisplayHeader = newValue;
    }

    public override string Key { get; set; } = "TooltipDisplayHeader";
    public override string Name { get; set; } = "添加插件名称";

    public override string HelpText { get; set; } =
        "是否在悬浮提示中所有修改内容的上方显示 [亚拉戈工具箱]？";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override string Version => "1.7.0.0";

    public TooltipDisplayHeaderSetting(ILogger<TooltipDisplayHeaderSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}