using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroCraftingLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page(LocalizationService.Ui("intro/crafting"), "Crafting",
            Paragraph(LocalizationService.Ui("A craft list takes what you want to make and breaks it into every intermediate item and raw material, then checks that against what you already own.")),
            Section(LocalizationService.Ui("What you get"),
                Bullet(LocalizationService.Ui("A full material tree, not just the immediate ingredients.")),
                Bullet(LocalizationService.Ui("What you already have, and which character or retainer is holding it.")),
                Bullet(LocalizationService.Ui("Where to buy or gather whatever is missing.")),
                Bullet(LocalizationService.Ui("Progress ticking down as you acquire things."))),
            Section(LocalizationService.Ui("Have a look"),
                OpenWindow<CraftsWindow>(LocalizationService.Ui("Open the crafts window")))
        );
    }
}