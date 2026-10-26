using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class ContextMenuOpenFishingLogSetting : BooleanSetting
{
    public ContextMenuOpenFishingLogSetting(ILogger<ContextMenuOpenFishingLogSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.OpenFishingLogContextMenu;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.OpenFishingLogContextMenu = newValue;
    }

    public override string Key { get; set; } = "OpenFishingLogContextMenu";
    public override string Name { get; set; } = "快捷菜单 - 打开钓鱼笔记";

    public override string WizardName { get; } = "打开钓鱼笔记";

    public override string HelpText { get; set; } =
        "是否为可钓鱼物品添加快捷菜单项以打开钓鱼笔记？";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ContextMenu;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override string Version { get; } = "1.11.0.2";
}