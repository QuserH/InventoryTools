using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayGlamourReadySetSetting : BooleanSetting
{
    public override bool DefaultValue { get; set; } = false;

    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipDisplayGlamourReadySet;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.TooltipDisplayGlamourReadySet = newValue;
    }

    public override string Key { get; set; } = "TooltipDisplayGlamourReadySet";
    public override string Name { get; set; } = "套装幻化信息";

    public override string WizardName { get; } = "套装幻化信息";

    public override string HelpText { get; set; } =
        "鼠标悬停物品时，显示它是否为幻化套装或套装的一部分，以及你在兵装库或收藏柜中已拥有的组件物品。";

    public override uint? Order { get; } = 0;

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.GlamourReadySet;
    public override string Version => "1.12.0.0";

    public TooltipDisplayGlamourReadySetSetting(ILogger<TooltipDisplayGlamourReadySetSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}
