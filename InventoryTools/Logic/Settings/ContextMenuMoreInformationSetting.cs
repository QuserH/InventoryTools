using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class ContextMenuMoreInformationSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = false;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.AddMoreInformationContextMenu;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.AddMoreInformationContextMenu = newValue;
        }

        public override string Key { get; set; } = "moreInfoContextMenu";
        public override string Name { get; set; } = "快捷菜单 - 更多信息（物品）";

        public override string WizardName { get; } = "更多信息";

        public override string HelpText { get; set; } =
            "是否在物品的右键/快捷菜单中添加更多信息菜单项？";

        public override SettingCategory SettingCategory { get; set; } = SettingCategory.ContextMenu;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";

        public ContextMenuMoreInformationSetting(ILogger<ContextMenuMoreInformationSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}