using InventoryTools.Localization;
﻿using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipCategoryBlacklistSetting : BooleanSetting
{
    public override bool DefaultValue { get; set; }
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipWhitelistBlacklist;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.TooltipWhitelistBlacklist = newValue;
    }

    public override string Key { get; set; } = "TooltipCategoryBlacklist";
    public override string Name { get; set; } = "悬浮提示类别黑名单";
    public override string HelpText { get; set; } = "勾选后将悬浮提示类别白名单转为黑名单。";
    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override string Version => "1.7.0.0";

    public TooltipCategoryBlacklistSetting(ILogger<TooltipCategoryBlacklistSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}