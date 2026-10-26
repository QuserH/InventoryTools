using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipGlamourReadySetDisplayModeSetting : ChoiceSetting<GlamourReadySetDisplayMode>
{
    public override GlamourReadySetDisplayMode DefaultValue { get; set; } = GlamourReadySetDisplayMode.Compact;

    public override GlamourReadySetDisplayMode CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipGlamourReadySetDisplayMode;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, GlamourReadySetDisplayMode newValue)
    {
        configuration.TooltipGlamourReadySetDisplayMode = newValue;
    }

    public override string Key { get; set; } = "TooltipGlamourReadySetDisplayMode";
    public override string Name { get; set; } = "显示模式";

    public override string WizardName { get; } = "显示模式";

    public override string HelpText { get; set; } =
        "幻化套装信息在悬浮提示中的显示方式。详细模式列出每件物品及其持有状态；紧凑模式显示汇总数量。";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.GlamourReadySet;

    public override Dictionary<GlamourReadySetDisplayMode, string> Choices => new()
    {
        { GlamourReadySetDisplayMode.Detailed, "详细（列出全部部件）" },
        { GlamourReadySetDisplayMode.Compact, "紧凑（数量摘要）" },
    };

    public override string Version => "1.12.0.0";

    public TooltipGlamourReadySetDisplayModeSetting(ILogger<TooltipGlamourReadySetDisplayModeSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}
