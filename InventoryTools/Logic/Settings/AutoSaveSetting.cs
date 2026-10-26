using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class AutoSaveSetting : BooleanSetting
    {
        private readonly PluginLogic _pluginLogic;

        public AutoSaveSetting(ILogger<AutoSaveSetting> logger, ImGuiService imGuiService, PluginLogic pluginLogic) : base(logger, imGuiService)
        {
            _pluginLogic = pluginLogic;
        }
        public override bool DefaultValue { get; set; } = true;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.AutoSave;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.AutoSave = newValue;
            _pluginLogic.ClearAutoSave();
        }

        public override string Key { get; set; } = "AutoSave";
        public override string Name { get; set; } = "自动保存背包/配置？";

        public override string WizardName { get; } = "自动保存背包？";

        public override string HelpText { get; set; } =
            "是否按设定间隔自动保存背包/配置？插件在游戏关闭和配置修改时会保存，但崩溃时不会，此功能可缓解该问题。";

        public override SettingCategory SettingCategory { get; set; } = SettingCategory.AutoSave;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";
    }
}