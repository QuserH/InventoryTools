using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class FilteringHelpLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("help/filtering", LocalizationService.Ui("Filtering"),
            Section(LocalizationService.Ui("Advanced search syntax"),
                Paragraph(LocalizationService.Ui("When creating a list, or when searching through the results of a list, you can use a series of operators to make your search more specific. Which operators are available depends on what you are searching against.")),
                Bullet(LocalizationService.Ui("!  - results that do not contain what is entered - text and numbers.")),
                Bullet(LocalizationService.Ui("<  - results with a value less than what is entered - numbers.")),
                Bullet(LocalizationService.Ui(">  - results with a value greater than what is entered - numbers.")),
                Bullet(LocalizationService.Ui(">= - results with a value greater than or equal to what is entered - numbers.")),
                Bullet(LocalizationService.Ui("<= - results with a value less than or equal to what is entered - numbers.")),
                Bullet(LocalizationService.Ui("=  - results with a value exactly equal to what is entered - text and numbers.")),
                Bullet(LocalizationService.Ui("&& and || - AND and OR respectively, used to chain operators together.")))
        );
    }
}