using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroListsLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page(LocalizationService.Ui("intro/lists"), "Lists",
            Paragraph(LocalizationService.Ui("Almost everything in the plugin hangs off a list. A list is a saved search over your items, and once you have one you can highlight its results in game, sort them, or track them.")),
            Section(LocalizationService.Ui("Three kinds"),
                Bullet(LocalizationService.Ui("Search list: find items across your inventories.")),
                Bullet(LocalizationService.Ui("Sort list: find items and work out where they should be moved to.")),
                Bullet(LocalizationService.Ui("Game item list: search every item in the game, not just the ones you own."))),
            Section(LocalizationService.Ui("Have a look"),
                Paragraph(LocalizationService.Ui("The items window is where lists live. There are sample lists you can install as well.")),
                OpenWindow<FiltersWindow>(LocalizationService.Ui("Open the items window")))
        );
    }
}
