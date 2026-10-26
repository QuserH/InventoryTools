using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class ColourRetainerListSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = true;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.ColorRetainerList;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.ColorRetainerList = newValue;
        }

        public override string Key { get; set; } = "ColourRetainerList";
        public override string Name { get; set; } = "雇员列表颜色名称？";
        public override string HelpText { get; set; } = "传唤铃列表中，若有物品需要整理或雇员背包中有相关物品时，是否将雇员名称着色？";
        public override SettingCategory SettingCategory { get; set; } = SettingCategory.Highlighting;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";

        public ColourRetainerListSetting(ILogger<ColourRetainerListSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}