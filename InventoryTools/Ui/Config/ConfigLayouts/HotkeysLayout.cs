using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class HotkeysLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("hotkeys", "Hotkeys",
            Section(LocalizationService.Ui("Toggle a window"),
                Paragraph(LocalizationService.Ui("Work anywhere, whether or not you are hovering something.")),
                Setting<HotKeyListsWindowSetting>("Lists"),
                Setting<HotkeyCraftWindowSetting>("Crafts"),
                Setting<HotkeyConfigWindowSetting>(LocalizationService.Ui("Configuration")),
                Setting<HotkeyMobWindowSetting>(LocalizationService.Ui("Mobs")),
                Setting<HotkeyDutiesWindowSetting>(LocalizationService.Ui("Duties")),
                Setting<HotkeyAirshipWindowSetting>(LocalizationService.Ui("Airships")),
                Setting<HotkeySubmarinesWindowSetting>(LocalizationService.Ui("Submarines")),
                Setting<HotkeyRetainerTasksWindowSetting>(LocalizationService.Ui("Retainer ventures"))),
            Section(LocalizationService.Ui("While hovering an item"),
                Paragraph(LocalizationService.Ui("These act on whatever item is under the cursor, so they only do anything while an item is hovered.")),
                Setting<HotkeyMoreInfoSetting>(LocalizationService.Ui("More information")),
                Setting<HotkeyOpenItemLogSetting>(LocalizationService.Ui("Whichever log applies")),
                Setting<HotkeyOpenCraftingLogSetting>(LocalizationService.Ui("Crafting log")),
                Setting<HotkeyOpenGatheringLogSetting>(LocalizationService.Ui("Gathering log")),
                Setting<HotkeyOpenFishingLogSetting>(LocalizationService.Ui("Fishing log")))
        );
    }
}