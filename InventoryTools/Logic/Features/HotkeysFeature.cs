using System.Collections.Generic;
using InventoryTools.Localization;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class HotkeysFeature : Feature
{
    public HotkeysFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/hotkeys", LocalizationService.Ui("Hotkeys"),
            Paragraph(LocalizationService.Ui("These hotkeys are optional. If you leave a field empty, that action has no hotkey.")),
            Section(LocalizationService.Ui("Show or hide a window"),
                Setting<HotKeyListsWindowSetting>(LocalizationService.Ui("Lists")),
                Setting<HotkeyCraftWindowSetting>(LocalizationService.Ui("Crafts")),
                Setting<HotkeyConfigWindowSetting>(LocalizationService.Ui("Configuration")),
                Setting<HotkeyMobWindowSetting>(LocalizationService.Ui("Mobs")),
                Setting<HotkeyDutiesWindowSetting>(LocalizationService.Ui("Duties")),
                Setting<HotkeyAirshipWindowSetting>(LocalizationService.Ui("Airships")),
                Setting<HotkeySubmarinesWindowSetting>(LocalizationService.Ui("Submarines")),
                Setting<HotkeyRetainerTasksWindowSetting>(LocalizationService.Ui("Retainer ventures"))),
            Section(LocalizationService.Ui("While the cursor is over an item"),
                Paragraph(LocalizationService.Ui("These hotkeys apply to the item under the cursor. If there is no item under the cursor, they do nothing.")),
                Setting<HotkeyMoreInfoSetting>(LocalizationService.Ui("More information")),
                Setting<HotkeyOpenItemLogSetting>(LocalizationService.Ui("The log that applies to the item")),
                Setting<HotkeyOpenCraftingLogSetting>(LocalizationService.Ui("Crafting log")),
                Setting<HotkeyOpenGatheringLogSetting>(LocalizationService.Ui("Gathering log")),
                Setting<HotkeyOpenFishingLogSetting>(LocalizationService.Ui("Fishing log")))
        );
    }
}
