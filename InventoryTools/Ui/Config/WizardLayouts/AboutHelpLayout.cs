using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class AboutHelpLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page("help/about", LocalizationService.Ui("About"),
            Paragraph(LocalizationService.Ui("This plugin is written in some of the free time that I have. It's a labour of love and I will hopefully be actively releasing updates for a while.")),
            Paragraph(LocalizationService.Ui("If you run into any issues please submit feedback via the plugin installer feedback button.")),
            Section(LocalizationService.Ui("Links"),
                Link(LocalizationService.Ui("Open the wiki"), "https://github.com/Critical-Impact/InventoryTools/wiki/1.-Overview"),
                Link(LocalizationService.Ui("Report a bug"), "https://github.com/Critical-Impact/InventoryTools/issues"))
        );
    }
}