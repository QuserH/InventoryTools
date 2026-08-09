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
        LocalizationService.Ui("Context Menu - Add to Curated List"),
        LocalizationService.Ui("Add a submenu to add the item to a curated list?"),
        false,
        SettingCategory.ContextMenu,
        SettingSubCategory.General,
        "1.7.0.21",
        logger,
        imGuiService)
    {
    }

    public override string WizardName { get; } = LocalizationService.Ui("Add to Curated List");

}