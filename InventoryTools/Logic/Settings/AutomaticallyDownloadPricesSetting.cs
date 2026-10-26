using CriticalCommonLib.MarketBoard;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class AutomaticallyDownloadPricesSetting : BooleanSetting
    {
        private readonly MarketCacheConfiguration _marketCacheConfiguration;

        public AutomaticallyDownloadPricesSetting(ILogger<AutomaticallyDownloadPricesSetting> logger, ImGuiService imGuiService, MarketCacheConfiguration marketCacheConfiguration) : base(logger, imGuiService)
        {
            _marketCacheConfiguration = marketCacheConfiguration;
        }
        public override bool DefaultValue { get; set; } = false;
        public override bool CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.AutomaticallyDownloadMarketPrices;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
        {
            configuration.AutomaticallyDownloadMarketPrices = newValue;
            _marketCacheConfiguration.AutoRequest = newValue;
        }

        public override string Key { get; set; } = "AutomaticallyDownloadPrices";
        public override string Name { get; set; } = "自动下载价格？";

        public override string WizardName { get; } = "下载价格数据";
        public override string HelpText { get; set; } = "在列表中查看价格数据时是否自动下载？";
        public override SettingCategory SettingCategory { get; set; } = SettingCategory.MarketBoard;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.Market;
        public override string Version => "1.7.0.0";
    }
}