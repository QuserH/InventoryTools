using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class AllowCrossCharacterSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = true;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.DisplayCrossCharacter;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.DisplayCrossCharacter = newValue;
        }

        public override string Key { get; set; } = "DisplayCrossCharacter";
        public override string Name { get; set; } = "在列表中显示跨角色背包？";
        public override string WizardName { get; } = "跨角色背包？";

        public override string HelpText { get; set; } =
            "未登录的角色及其关联雇员是否可用于查看/搜索/筛选？每个列表可配置来源/目标以包含/排除与当前登录角色相关的雇员/部队。";

        public override SettingCategory SettingCategory { get; set; } = SettingCategory.Lists;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";

        public AllowCrossCharacterSetting(ILogger<AllowCrossCharacterSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}