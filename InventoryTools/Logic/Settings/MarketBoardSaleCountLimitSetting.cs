using System;
using CriticalCommonLib.MarketBoard;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class MarketBoardSaleCountLimitSetting : IntegerSetting
    {
        public MarketBoardSaleCountLimitSetting(ILogger<MarketBoardSaleCountLimitSetting> logger, ImGuiService imGuiService, IHostedUniversalisConfiguration universalisConfiguration) : base(logger, imGuiService)
        {
            _universalisConfiguration = universalisConfiguration;
        }
        
        private readonly IHostedUniversalisConfiguration _universalisConfiguration;
        public override int DefaultValue { get; set; } = 7;
        public override int CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.MarketSaleHistoryLimit;
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, int newValue)
        {
            newValue = Math.Min(30, newValue);
            newValue = Math.Max(1, newValue);
            _universalisConfiguration.SaleHistoryLimit = newValue;
            configuration.MarketSaleHistoryLimit = newValue;
        }

        public override string Key { get; set; } = "MBSaleCountLimit";
        public override string Name { get; set; } = "市场布告板成交记录天数";

        public override string WizardName { get; } = "成交记录上限";

        public override string HelpText { get; set; } =
            "计算物品成交总数时，回溯多少天内的成交数据。修改后现有数据不会被清除，你需要手动请求刷新市场布告板价格，或等待市场布告板自动刷新。";

        public override SettingCategory SettingCategory { get; set; } = SettingCategory.MarketBoard;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.Market;
        public override string Version => "1.7.0.0";
    }
}