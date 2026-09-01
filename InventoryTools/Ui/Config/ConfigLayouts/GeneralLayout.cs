using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class GeneralLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("general", "General",
            Section("Saving",
                Paragraph(LocalizationService.Ui("Allagan Tools keeps its own record of your inventories. These control how often that record is written to disk.")),
                Setting<AutoSaveSetting>(LocalizationService.Ui("Save automatically")),
                EnabledBy<AutoSaveSetting>(
                    Setting<AutoSaveTimeSetting>(LocalizationService.Ui("How often"))),
                Setting<PersistDataSetting>()),
            Section("Integrations",
                Paragraph(LocalizationService.Ui("Ways to reach Allagan Tools from outside its own windows.")),
                Setting<AddTitleMenuButtonSetting>(LocalizationService.Ui("Add a button to the title menu")),
                Setting<CompendiumWotsitSetting>(LocalizationService.Ui("List compendium windows in Wotsit"))),
            Section("Integrations",
                Paragraph(LocalizationService.Ui("Ways to reach Allagan Tools from outside its own windows.")),
                Setting<ActiveCraftListSetting>())
        );
    }
}