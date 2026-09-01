using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroDefaultsLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page(LocalizationService.Ui("intro/defaults"), "Defaults",
            Paragraph(LocalizationService.Ui("By default, the plugin is configured with a default set of features enabled.")),
            Paragraph(LocalizationService.Ui("The next screens will show you the settings for the most commonly used features.")),
            Paragraph(LocalizationService.Ui("Hover the ? icons to get further information about what each setting does."))
        );
    }
}