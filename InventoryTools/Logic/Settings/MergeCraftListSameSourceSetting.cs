using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class MergeCraftListSameSourceSetting : BooleanSetting
{
    public MergeCraftListSameSourceSetting(ILogger<MergeCraftListSameSourceSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = true;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.MergeCraftListSameSource;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.MergeCraftListSameSource = newValue;
    }

    public override string Key { get; set; } = "MergeCraftListSameSource";
    public override string Name { get; set; } = "合并角色、来源、道具名称均一致的数量？";
    public override string HelpText { get; set; } =
        "是否将制作列表下方“雇员/背包中的物品”面板中，同一角色同一雇员持有的同种道具堆叠合并为一行？合并后不再显示具体背包位置；是否连同NQ和HQ一起合并由下一项决定。";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.General;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override uint? Order => 1;
    public override string Version => "1.7.0.0";
}
