using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class ListBasicsHelpLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("help/list-basics", LocalizationService.Ui("List Basics"),
            Paragraph(LocalizationService.Ui("Lists are the core way the plugin lets you view the items you are looking for, or are attempting to sort. There are currently 3 types of list that can be created.")),
            Section(LocalizationService.Ui("Search List"),
                Paragraph(LocalizationService.Ui("Allows you to search for specific items across all your inventories. If you just need to find an item but don't want help sorting it, this is the list type you want.")),
                Paragraph(LocalizationService.Ui("Example uses:")),
                Bullet(LocalizationService.Ui("Finding materials for a craft.")),
                Bullet(LocalizationService.Ui("Finding a housing item you put somewhere.")),
                Bullet(LocalizationService.Ui("Seeing how much an item you just picked up is worth.")),
                Bullet(LocalizationService.Ui("Seeing if a specific item is already in your glamour chest or armoire.")),
                Bullet(LocalizationService.Ui("Checking your retainers' equipment without going to a retainer bell.")),
                Bullet(LocalizationService.Ui("Checking if any items you have can go into the armoire."))),
            Section(LocalizationService.Ui("Sort List"),
                Paragraph(LocalizationService.Ui("Builds on the search list, but also lets you pick where you want the items to be sorted. It'll attempt to show you the most optimised plan for storing the items in the destinations you pick.")),
                Paragraph(LocalizationService.Ui("Example uses:")),
                Bullet(LocalizationService.Ui("Putting away materials after a craft without having them double up.")),
                Bullet(LocalizationService.Ui("Storing items above a certain item level in your chocobo saddlebag for later.")),
                Bullet(LocalizationService.Ui("Finding items that are unique to your free company chest and putting them there."))),
            Section(LocalizationService.Ui("Game Item List"),
                Paragraph(LocalizationService.Ui("Lets you search across all the items that exist within the game's catalogue.")),
                Paragraph(LocalizationService.Ui("Example uses:")),
                Bullet(LocalizationService.Ui("Searching for glamours.")),
                Bullet(LocalizationService.Ui("Seeing what mounts and minions you haven't obtained.")),
                Bullet(LocalizationService.Ui("Tracking the prices of all the items within the game.")))
        );
    }
}