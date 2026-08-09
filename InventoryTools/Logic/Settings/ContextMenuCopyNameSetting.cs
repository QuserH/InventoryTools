using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class ContextMenuCopyNameSetting : GenericBooleanSetting
{
    public ContextMenuCopyNameSetting(ILogger<ContextMenuCopyNameSetting> logger,
        ImGuiService imGuiService) : base("CopyNameContextMenu",
        LocalizationService.Ui("Context Menu - Copy Name"),
        LocalizationService.Ui("Copies the item's name to the clipboard."),
        false,
        SettingCategory.ContextMenu,
        SettingSubCategory.General,
        "15.0.6",
        logger,
        imGuiService)
    {
    }

    public override string WizardName => LocalizationService.Ui("Copy Name");
}