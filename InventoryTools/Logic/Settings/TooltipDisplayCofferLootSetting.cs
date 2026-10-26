using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayCofferLootSetting : BooleanSetting
{
    public override bool DefaultValue { get; set; } = false;

    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipDisplayCofferLoot;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.TooltipDisplayCofferLoot = newValue;
    }

    public override string Key { get; set; } = "TooltipDisplayCofferLoot";
    public override string Name { get; set; } = "宝箱物品信息";

    public override string WizardName { get; } = "宝箱物品信息";

    public override string HelpText { get; set; } =
        "鼠标悬停宝匣、卡包或战利品容器时，显示其中可能的战利品你已拥有多少。";

    public override uint? Order { get; } = 0;

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.CofferLoot;
    public override string Version => "1.12.0.0";

    public TooltipDisplayCofferLootSetting(ILogger<TooltipDisplayCofferLootSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}
