using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class MarketBoardLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("marketboard", LocalizationService.Ui("Market Board"),
            Paragraph(LocalizationService.Ui("Market prices come from Universalis rather than the game, so they are fetched and cached rather than read live.")),
            Section(LocalizationService.Ui("Worlds to price"),
                Paragraph(LocalizationService.Ui("These combine into a single list, the extra worlds below are added to your home and current world rather than replacing them.")),
                Setting<MarketBoardUseHomeWorldSetting>(LocalizationService.Ui("Home world")),
                Setting<MarketBoardUseActiveWorldSetting>(LocalizationService.Ui("Current world")),
                Setting<MarketBoardExtraWorldsSetting>(LocalizationService.Ui("Additional worlds"))),
            Section(LocalizationService.Ui("Downloading"),
            Paragraph(LocalizationService.Ui("Should pricing data be downloaded automatically? If not-enabled the 'Refresh Market Prices' button must be pressed to download pricing for items.")),
                Setting<AutomaticallyDownloadPricesSetting>(LocalizationService.Ui("Download prices automatically")),
                Setting<MarketRefreshTimeHoursSetting>(LocalizationService.Ui("Keep prices for (hours)")),
                Setting<MarketBoardSaleCountLimitSetting>(LocalizationService.Ui("Sale history window (days)")))
        );
    }
}