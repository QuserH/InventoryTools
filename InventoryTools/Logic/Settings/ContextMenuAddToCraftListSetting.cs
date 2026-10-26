using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class ContextMenuAddToCraftListSetting : BooleanSetting
    {
        public ContextMenuAddToCraftListSetting(ILogger<ContextMenuAddToCraftListSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        
        public override bool DefaultValue { get; set; } = false;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.AddToCraftListContextMenu;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.AddToCraftListContextMenu = newValue;
        }

        public override string Key { get; set; } = "addToCraftListContextMenu";
        public override string Name { get; set; } = "快捷菜单 - 添加到制作列表";

        public override string WizardName { get; } = "添加到制作列表";

        public override string HelpText { get; set; } =
            "是否添加子菜单以将物品添加到制作列表？";

        public override SettingCategory SettingCategory { get; set; } = SettingCategory.ContextMenu;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";
    }
}