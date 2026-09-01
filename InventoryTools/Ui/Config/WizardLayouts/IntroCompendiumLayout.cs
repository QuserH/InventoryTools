using InventoryTools.Compendium.Windows;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.WizardLayouts;

public class IntroCompendiumLayout : ContentLayout
{
    public override PageLayout Build()
    {
        return Page(LocalizationService.Ui("intro/compendium"), "Compendium",
            Paragraph(LocalizationService.Ui("The compendium is a browsable reference for the game's content, built from the same data the plugin uses everywhere else. If you want to know what something is, where it comes from or what drops it, this is where to look.")),
            Section(LocalizationService.Ui("What's in it"),
                Bullet(LocalizationService.Ui("Items, quests and achievements.")),
                Bullet(LocalizationService.Ui("Mounts, minions and glamour sets.")),
                Bullet(LocalizationService.Ui("Duties, leves, beast tribes and custom deliveries.")),
                Bullet(LocalizationService.Ui("Relic weapons and tools, master recipe books, folklore tomes and soul crystals.")),
                Bullet(LocalizationService.Ui("NPCs, territories, and airship and submarine routes.")),
                Bullet(LocalizationService.Ui("Classes, gearsets, chocobo items and more besides."))),
            Section(LocalizationService.Ui("Have a look"),
                Paragraph(LocalizationService.Ui("Each entry links through to everything related to it, so you can follow a chain from an item to the duty that drops it to the NPC that sells the rest.")),
                OpenWindow<CompendiumTypesWindow>(LocalizationService.Ui("Open the compendium")),
                Paragraph(LocalizationService.Ui("It also lives under the Compendium menu in the items and crafts windows, or the /compendium command.")))
        );
    }
}