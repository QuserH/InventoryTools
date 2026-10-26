using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class AddTitleMenuButtonSetting : BooleanSetting
{
    public AddTitleMenuButtonSetting(ILogger<AddTitleMenuButtonSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.AddTitleMenuButton;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.AddTitleMenuButton = newValue;
    }

    public override string Key { get; set; } = "AddTitleMenuButton";
    public override string Name { get; set; } = "添加标题栏按钮？";

    public override string HelpText { get; set; } =
        "在标题界面菜单旁添加一个按钮，让你在未登录时也能打开亚拉戈工具箱。";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.TitleMenuButtons;
    public override SettingSubCategory SettingSubCategory => SettingSubCategory.General;
    public override string Version => "1.7.0.0"; 
}