using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class WindowsAndListsLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("windows-lists", LocalizationService.Ui("Windows & Lists"),
            Section(LocalizationService.Ui("Layout"),
                Paragraph(LocalizationService.Ui("Control how various windows are laid out.")),
                Setting<CraftWindowLayoutSetting>(LocalizationService.Ui("Craft window")),
                Setting<FiltersWindowLayoutSetting>(LocalizationService.Ui("Items window")),
                Setting<ShowFiltersTabSetting>(),
                Setting<CompendiumRowHeightSetting>(LocalizationService.Ui("Compendium row height"))),
            Section("Auto-Switch",
                Paragraph(LocalizationService.Ui("When switching between lists in the UI, if highlighting is on should we automatically switch highlighting to that list?")),
                Setting<SwitchFiltersAutomaticallySetting>(),
                Setting<SwitchCraftListsAutomaticallySetting>()),
            Section(LocalizationService.Ui("Ignore escape"),
                Paragraph(LocalizationService.Ui("Windows that should stay open when you press escape.")),
                Setting<CraftWindowIgnoreEscapeSetting>(LocalizationService.Ui("Craft window")),
                Setting<FiltersWindowIgnoreEscapeSetting>(LocalizationService.Ui("Items window")),
                Setting<ItemWindowIgnoreEscapeSetting>(LocalizationService.Ui("Item window")),
                Setting<FilterWindowIgnoreEscapeSetting>(LocalizationService.Ui("List window")))
        );
    }
}