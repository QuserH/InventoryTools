using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class HighlightingLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("highlighting", "Highlighting",
            Paragraph(LocalizationService.Ui("Highlighting tints inventory slots and bag tabs when a list matches the items inside them. These are the defaults, individual lists can override most of them.")),
            Section(LocalizationService.Ui("Active lists"),
                Paragraph(LocalizationService.Ui("Highlighting is generally controlled by hitting the 'Highlight' button within the various windows and via slash commands, you can toggle highlight on/off here as well.")),
                Setting<WindowFilterSetting>(),
                Setting<BackgroundFilterSetting>(),
                Setting<SaveBackgroundFilterSetting>(LocalizationService.Ui("Remember the choice between sessions"))
            ),
            Section(LocalizationService.Ui("When to highlight"),
                Setting<HighlightWhenSetting>(),
                Setting<InvertHighlightingSetting>(),
                Setting<InvertTabHighlightingSetting>()),
            Section(LocalizationService.Ui("Colours"),
                Setting<HighlightColourSetting>(LocalizationService.Ui("Matched item colour")),
                Setting<TabHighlightColourSetting>(LocalizationService.Ui("Matching tab colour"))),
            Section(LocalizationService.Ui("Destinations"),
                Paragraph(LocalizationService.Ui("Sort lists know where items are meant to end up. The destination bag can be highlighted alongside the source so you can see both ends of a move at once.")),
                Setting<HighlightDestinationSetting>(),
                EnabledBy<HighlightDestinationSetting>(
                    Setting<HighlightDestinationEmptySetting>(),
                    Setting<InvertDestinationHighlightingSetting>(),
                    Setting<HighlightDestinationColourSetting>(LocalizationService.Ui("Destination colour")))),
            Section(LocalizationService.Ui("Retainer list"),
                Paragraph(LocalizationService.Ui("The summoning bell list can be annotated when a retainer holds items one of your lists cares about, so you know which to open without checking each one.")),
                Setting<ColourRetainerListSetting>(LocalizationService.Ui("Colour retainer names")),
                EnabledBy<ColourRetainerListSetting>(
                    Setting<RetainerListColourSetting>(LocalizationService.Ui("Name colour"))),
                Setting<ShowItemNumberRetainerListSetting>(LocalizationService.Ui("Show item counts"))),
            Section(LocalizationService.Ui("Shop highlighting"),
                Paragraph(LocalizationService.Ui("While a vendor window is open, shop items matched by a list can be highlighted, and the vendors that sell them can be marked out in the world.")),
                Setting<ShopHighlightingDisableItemsSetting>(LocalizationService.Ui("Dim items that don't match")),
                Setting<ShopHighlightingNpcSetting>(LocalizationService.Ui("Highlight vendor NPCs in the world")),
                EnabledBy<ShopHighlightingNpcSetting>(
                    Setting<ShopHighlightingNpcColorSetting>(LocalizationService.Ui("Highlight colour")),
                    Setting<ShopHighlightingNpcNameplateIconSetting>(LocalizationService.Ui("Show an icon on their nameplate"))))
        );
    }
}