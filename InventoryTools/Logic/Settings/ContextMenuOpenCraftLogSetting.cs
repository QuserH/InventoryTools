using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class ContextMenuOpenCraftingLogSetting : BooleanSetting
{
    public ContextMenuOpenCraftingLogSetting(ILogger<ContextMenuOpenCraftingLogSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.OpenCraftingLogContextMenu;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.OpenCraftingLogContextMenu = newValue;
    }

    public override string Key { get; set; } = "OpenCraftingLogContextMenu";
    public override string Name { get; set; } = "快捷菜单 - 打开制作笔记";

    public override string WizardName { get; } = "打开制作笔记";

    public override string HelpText { get; set; } =
        "是否为可制作物品添加快捷菜单项以打开制作笔记？";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ContextMenu;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override string Version { get; } = "1.11.0.2";
}