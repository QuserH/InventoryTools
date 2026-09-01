using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class HistoryLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("history", "History",
            Paragraph(LocalizationService.Ui("Records items moving into, out of and around your inventories so you can look back at what changed. History lists then read from that record.")),
            Paragraph(LocalizationService.Ui("Without this enabled, History type lists will not function.")),
            Section(LocalizationService.Ui("Tracking"),
                Setting<HistoryEnabledSetting>(LocalizationService.Ui("Track inventory changes")),
                EnabledBy<HistoryEnabledSetting>(
                    Setting<HistoryTrackEventsSetting>(LocalizationService.Ui("Events worth recording"))))
        );
    }
}