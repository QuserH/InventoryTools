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
    public override string Name { get; } = "市场布告板";
    public override string Description { get; } =
        "配置市场布告板整合。将按设定计时器从 Universalis 下载数据，使你能够按多个服务器的物品最低价和平均价筛选。";
}