using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class SwitchFiltersAutomaticallySetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = true;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.SwitchFiltersAutomatically;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.SwitchFiltersAutomatically = newValue;
        }

        public override string Key { get; set; } = "SwitchFiltersAutomatically";
        public override string Name { get; set; } = "自动切换列表？";

        public override string HelpText { get; set; } =
            "查看不同列表时，高亮是否自动切换到当前查看的列表？仅当高亮已启用时才会切换。";
        public override SettingCategory SettingCategory { get; set; } = SettingCategory.Lists;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";

        public SwitchFiltersAutomaticallySetting(ILogger<SwitchFiltersAutomaticallySetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}