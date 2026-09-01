using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class CraftOverlayLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("craft-overlay", LocalizationService.Ui("Craft Overlay"),
            Paragraph(LocalizationService.Ui("A compact panel that follows your active craft list while you are in the game, so you can see what is still needed without opening a window.")),
            Section(LocalizationService.Ui("Display"),
                Setting<CraftOverlayMaxExpandedItemsSetting>(LocalizationService.Ui("Items shown when expanded")),
                Setting<CraftOverlayHideSetting>(LocalizationService.Ui("Hide during duties and cutscenes"))),
            Section(LocalizationService.Ui("Behaviour"),
                Setting<CraftOverlayRememberStateSetting>(LocalizationService.Ui("Stay open across reloads")))
        );
    }
}