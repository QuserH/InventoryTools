using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroInventoriesLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page(LocalizationService.Ui("intro/inventories"), "Your items",
            Paragraph(LocalizationService.Ui("The plugin can only see an inventory after the game has shown it to you at least once. That is a limit of how the game sends its data, not a setting you can turn on.")),
            Section(LocalizationService.Ui("If something is missing"),
                Paragraph(LocalizationService.Ui("Open it in game once and it will be remembered from then on. Most commonly:")),
                Bullet(LocalizationService.Ui("Retainers: talk to a summoning bell and open each retainer's inventory.")),
                Bullet(LocalizationService.Ui("Free company chest: open every tab you care about.")),
                Bullet(LocalizationService.Ui("Glamour chest and armoire.")),
                Bullet(LocalizationService.Ui("Chocobo saddlebag."))),
            Section(LocalizationService.Ui("After that"),
                Paragraph(LocalizationService.Ui("The plugin keeps its own copy, so you can search your retainers' contents from anywhere without going back to a bell.")))
        );
    }
}
