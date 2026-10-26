using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class HighlightDestinationEmptySetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = false;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.HighlightDestinationEmpty;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.HighlightDestinationEmpty = newValue;
        }

        public override string Key { get; set; } = "HighlightEmptyDestination";
        public override string Name { get; set; } = "高亮空目标？";

        public override string HelpText { get; set; } =
            "高亮目标时，是仅高亮目标中已存在的物品，还是也高亮空格？";

        public override SettingCategory SettingCategory { get; set; } = SettingCategory.Highlighting;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";

        public HighlightDestinationEmptySetting(ILogger<HighlightDestinationEmptySetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}