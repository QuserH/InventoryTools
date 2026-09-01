using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class ContextMenuLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("context-menu", LocalizationService.Ui("Context Menu"),
            Paragraph(LocalizationService.Ui("Entries Allagan Tools adds to the game's own right-click menus. Each one costs a line in menus you already use, so they are individually switchable.")),
            Section("Lists",
                Setting<ContextMenuAddToCraftListSetting>(LocalizationService.Ui("Add to craft list")),
                Setting<ContextMenuAddToActiveCraftListSetting>(LocalizationService.Ui("Add to the active craft list")),
                Setting<ContextMenuAddToCuratedListSetting>(LocalizationService.Ui("Add to curated list")),
                Setting<ContextMenuAddToFavouritesSetting>(LocalizationService.Ui("Add or remove from favourites"))),
            Section(LocalizationService.Ui("More information"),
                Paragraph(LocalizationService.Ui("Opens the Allagan Tools information window for whatever was right-clicked.")),
                Setting<ContextMenuMoreInformationSetting>("Items"),
                Setting<ContextMenuMoreInformationNpcsSetting>("NPCs"),
                Setting<ContextMenuMoreInformationMonstersSetting>("Monsters")),
            Section(LocalizationService.Ui("Open in game"),
                Paragraph(LocalizationService.Ui("Shortcuts to open the various logs within the game.")),
                Setting<ContextMenuOpenCraftingLogSetting>(LocalizationService.Ui("Crafting log")),
                Setting<ContextMenuOpenGatheringLogSetting>(LocalizationService.Ui("Gathering log")),
                Setting<ContextMenuOpenFishingLogSetting>(LocalizationService.Ui("Fishing log"))),
            Section("Search",
                Paragraph(LocalizationService.Ui("The search locations also apply to searches started from Allagan Tools' own menus, not just this context menu entry.")),
                Paragraph(LocalizationService.Ui("This search is more expansive than the game's default search functionality as it searches across every character the plugin knows about.")),
                Setting<ContextMenuItemSearchSetting>(LocalizationService.Ui("Search for this item")),
                Paragraph(LocalizationService.Ui("If you want to limit the locations it searches, configure the search scope below.")),
                Setting<ContextMenuItemSearchScopeSetting>(LocalizationService.Ui("Search these locations"))),
            Section("Other",
                Setting<ContextMenuCopyNameSetting>(LocalizationService.Ui("Copy item name")))
        );
    }
}