using System.Numerics;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class HighlightColourSetting : ColorSetting
    {
        public override Vector4 DefaultValue { get; set; } = new(0.007f, 0.008f,
            0.007f, 0.212f);
        public override Vector4 CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.HighlightColor;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, Vector4 newValue)
        {
            configuration.HighlightColor = newValue;
        }

        public override string Key { get; set; } = "HighlightColour";
        public override string Name { get; set; } = "高亮颜色";
        public override string HelpText { get; set; } = "高亮物品的颜色。";
        public override SettingCategory SettingCategory { get; set; } = SettingCategory.Highlighting;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.Colours;
        public override string Version => "1.7.0.0";

        public HighlightColourSetting(ILogger<HighlightColourSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}