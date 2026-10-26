using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class SwitchCraftListsAutomaticallySetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = false;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.SwitchCraftListsAutomatically;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.SwitchCraftListsAutomatically = newValue;
        }

        public override string Key { get; set; } = "SwitchCraftListsAutomatically";
        public override string Name { get; set; } = "自动切换制作列表？";

        public override string HelpText { get; set; } =
            "在制作列表之间切换时，是否自动更改当前制作列表？仅当已选择当前制作列表时才会更改。";
        public override SettingCategory SettingCategory { get; set; } = SettingCategory.Lists;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";

        public SwitchCraftListsAutomaticallySetting(ILogger<SwitchCraftListsAutomaticallySetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}