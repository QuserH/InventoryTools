using System.Collections.Generic;
using InventoryTools.Localization;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class MarketboardIntegrationFeature : Feature
{
    public MarketboardIntegrationFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/marketboard", LocalizationService.Ui("Market Board"),
            Paragraph(LocalizationService.Ui("Prices come from Universalis, not from the game. The plugin downloads the prices and keeps a local copy. When this is on, prices are automatically downloaded. If not on, you must hit 'Refresh Market Prices' each time you want to get updated pricing on items.")),
            Setting<AutomaticallyDownloadPricesSetting>(LocalizationService.Ui("Download prices automatically")),
            Setting<MarketRefreshTimeHoursSetting>(LocalizationService.Ui("Keep prices for this many hours")),
            Setting<MarketBoardSaleCountLimitSetting>(LocalizationService.Ui("Days of sale history to include"))
        );
    }
}
