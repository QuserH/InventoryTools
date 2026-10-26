using InventoryTools.Localization;
﻿using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class EnableTetrisSetting : BooleanSetting
{
    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TetrisEnabled;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.TetrisEnabled = newValue;
    }

    public override string Key { get; set; } = "TetrisEnabled";
    public override string Name { get; set; } = "启用俄罗斯方块？";

    public override string HelpText { get; set; } =
        "是否启用俄罗斯方块？启用后插件汉堡菜单中将显示俄罗斯方块选项。";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.Misc;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override string Version => "1.7.0.0";

    public EnableTetrisSetting(ILogger<EnableTetrisSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}