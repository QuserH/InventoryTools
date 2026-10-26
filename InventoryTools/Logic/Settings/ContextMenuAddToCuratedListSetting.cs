using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class ContextMenuAddToCuratedListSetting : GenericBooleanSetting
{
    public ContextMenuAddToCuratedListSetting(ILogger<ContextMenuAddToCuratedListSetting> logger,
        ImGuiService imGuiService) : base("AddToCuratedListContextMenu",
        "快捷菜单 - 添加到精选列表",
        "是否添加子菜单以将物品添加到精选列表？",
        false,
        SettingCategory.ContextMenu,
        SettingSubCategory.General,
        "1.7.0.21",
        logger,
        imGuiService)
    {
    }

    public override string WizardName { get; } = "添加到精选列表";

}