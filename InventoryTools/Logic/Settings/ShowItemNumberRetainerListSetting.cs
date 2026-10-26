using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class ShowItemNumberRetainerListSetting : BooleanSetting
    {
        public override bool DefaultValue { get; set; } = true;

        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.ShowItemNumberRetainerList;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.ShowItemNumberRetainerList = newValue;
        }

        public override string Key { get; set; } = "ShowItemNumberRetainerList";
        public override string Name { get; set; } = "在雇员列表中显示道具种类数？";

        public override string HelpText { get; set; } =
            "传唤铃列表中雇员名称旁是否显示待整理或背包中符合筛选条件的道具种类数？";

        public override SettingCategory SettingCategory { get; set; } = SettingCategory.Highlighting;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
        public override string Version => "1.7.0.0";

        public ShowItemNumberRetainerListSetting(ILogger<ShowItemNumberRetainerListSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}