using System.Collections.Generic;
using InventoryTools.Localization;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class ContextMenuFeature : Feature
{
    public ContextMenuFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/context-menu", LocalizationService.Ui("Context Menus"),
            Paragraph(LocalizationService.Ui("The plugin can add these entries to item's right-click menus.")),
            Section(LocalizationService.Ui("More information"),
                Setting<ContextMenuMoreInformationSetting>(LocalizationService.Ui("Items")),
                Setting<ContextMenuMoreInformationNpcsSetting>(LocalizationService.Ui("NPCs")),
                Setting<ContextMenuMoreInformationMonstersSetting>(LocalizationService.Ui("Monsters"))),
            Section(LocalizationService.Ui("Open a game log"),
                Setting<ContextMenuOpenCraftingLogSetting>(LocalizationService.Ui("Crafting log")),
                Setting<ContextMenuOpenGatheringLogSetting>(LocalizationService.Ui("Gathering log")),
                Setting<ContextMenuOpenFishingLogSetting>(LocalizationService.Ui("Fishing log"))),
            Section(LocalizationService.Ui("Lists"),
                Setting<ContextMenuAddToCraftListSetting>(LocalizationService.Ui("Add to a craft list")),
                Setting<ContextMenuAddToActiveCraftListSetting>(LocalizationService.Ui("Add to the active craft list")),
                Setting<ContextMenuAddToCuratedListSetting>(LocalizationService.Ui("Add to a curated list")),
                Setting<ContextMenuAddToFavouritesSetting>(LocalizationService.Ui("Add to or remove from favourites"))),
            Section(LocalizationService.Ui("Other"),
                Setting<ContextMenuCopyNameSetting>(LocalizationService.Ui("Copy the item name")))
        );
    }
}
