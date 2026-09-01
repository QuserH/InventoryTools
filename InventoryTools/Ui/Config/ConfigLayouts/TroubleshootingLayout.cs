using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class TroubleshootingLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("troubleshooting", LocalizationService.Ui("Troubleshooting"),
            Paragraph(LocalizationService.Ui("Timings that only need changing if you are seeing a specific symptom. The defaults suit almost everyone.")),
            Section(LocalizationService.Ui("Acquisition tracker"),
                Paragraph(LocalizationService.Ui("The acquisition tracker watches what you craft, gather and loot so craft lists can tick themselves off. Both values are in seconds, and raising them trades responsiveness for reliability on a slow connection or a busy machine.")),
                Setting<AcquisitionTrackerLoginDelaySetting>(LocalizationService.Ui("Wait this long after login before scanning")),
                Setting<AcquisitionTrackerPersistStateSetting>(LocalizationService.Ui("Keep tracking this long after you stop")))
        );
    }
}
