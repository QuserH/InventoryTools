using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class ShowFiltersTabSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = true;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.ShowFilterTab;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.ShowFilterTab = newValue;
        }

        public override string Key { get; set; } = "ShowFiltersTab";
        public override string Name { get; set; } = "显示全部列表标签页？";

        public override string HelpText { get; set; } =
            "主窗口是否显示名为全部列表的标签页，将所有可用列表集中到一个标签页？";

        public override SettingCategory SettingCategory { get; set; } = SettingCategory.Windows;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";

        public ShowFiltersTabSetting(ILogger<ShowFiltersTabSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}