using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class ShowTooltipsSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = true;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.DisplayTooltip;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.DisplayTooltip = newValue;
        }

        public override string Key { get; set; } = "ShowTooltips";
        public override string Name { get; set; } = "启用悬浮提示调整？";

        public override string HelpText { get; set; } =
            "启用/禁用插件的整个悬浮提示修改系统。关闭后不会对你的物品悬浮提示进行任何更改。";

        public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";

        public ShowTooltipsSetting(ILogger<ShowTooltipsSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}