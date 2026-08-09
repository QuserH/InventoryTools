using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Features;

public class MarketboardIntegrationFeature : Feature
{
    public MarketboardIntegrationFeature(IEnumerable<ISetting> settings) : base(new[]
        {
            typeof(AutomaticallyDownloadPricesSetting),
            typeof(MarketRefreshTimeHoursSetting),
            typeof(MarketBoardSaleCountLimitSetting),
        },
        settings)
    {
    }
    public override string Name { get; } = LocalizationService.Ui("Marketboard");
    public override string Description { get; } =
        LocalizationService.Ui("Configure the marketboard integration. This downloads data from Universalis on a set timer, allowing you to filter against the minimum and average prices of items across multiple servers.");
}