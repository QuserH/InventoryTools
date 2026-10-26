using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class TooltipColorSetting : GameColorSetting
    {
        public TooltipColorSetting(ILogger<TooltipColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base(logger, imGuiService, uiColorSheet)
        {
        }
        public override uint? DefaultValue { get; set; } = null;
        public override uint? CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.TooltipColor;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, uint? newValue)
        {
            configuration.TooltipColor = newValue;
        }

        public override string Key { get; set; } = "TooltipColor";
        public override string Name { get; set; } = "文字颜色";
        public override string HelpText { get; set; } = "添加到物品悬浮提示中的文字颜色。可在悬浮提示设置中为每个模块单独设置颜色。";
        public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.Visuals;
        public override string Version => "1.7.0.0";
    }
}