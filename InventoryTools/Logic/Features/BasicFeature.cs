using System.Collections.Generic;
using InventoryTools.Localization;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class BasicFeature : Feature
{
    public BasicFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/basic", LocalizationService.Ui("Basics"),
            Paragraph(LocalizationService.Ui("These settings apply to the whole plugin. You can change all of them later in the settings window.")),
            Setting<AutoSaveSetting>(LocalizationService.Ui("Save inventories automatically")),
            Setting<HistoryEnabledSetting>(LocalizationService.Ui("Enable historical item tracking")),
            Setting<AddTitleMenuButtonSetting>(LocalizationService.Ui("Add a button to the game's title menu"))
        );
    }
}
