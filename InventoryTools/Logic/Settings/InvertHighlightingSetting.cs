using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class InvertHighlightingSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = true;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.InvertHighlighting;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.InvertHighlighting = newValue;
        }

        public override string Key { get; set; } = "InvertHighlighting";
        public override string Name { get; set; } = "反转高亮？";

        public override string HelpText { get; set; } =
            "是否改为高亮不匹配列表的全部物品？可在列表配置中覆盖。";

        public override SettingCategory SettingCategory { get; set; } = SettingCategory.Highlighting;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";

        public InvertHighlightingSetting(ILogger<InvertHighlightingSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}