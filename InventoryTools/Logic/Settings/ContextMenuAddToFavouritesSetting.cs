using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class ContextMenuAddToFavouritesSetting : GenericBooleanSetting
{
    public ContextMenuAddToFavouritesSetting(ILogger<ContextMenuAddToFavouritesSetting> logger,
        ImGuiService imGuiService) : base("AddToFavouritesContextMenu",
        "快捷菜单 - 添加/取消收藏",
        "是否添加子菜单以添加/取消物品的收藏？",
        false,
        SettingCategory.ContextMenu,
        SettingSubCategory.General,
        "1.13.1.9",
        logger,
        imGuiService)
    {
    }

    public override string WizardName { get; } = "添加/取消收藏";

}