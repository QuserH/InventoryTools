using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib.MarketBoard;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using InventoryTools.Localization;

using InventoryTools.Extensions;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters
{
    public class MarketBoardSaleCountFilter : StringFilter
    {
        private readonly InventoryToolsConfiguration _configuration;
        private readonly ICharacterMonitor _characterMonitor;
        private readonly IMarketCache _marketCache;

        public MarketBoardSaleCountFilter(ILogger<MarketBoardSaleCountFilter> logger, ImGuiService imGuiService, InventoryToolsConfiguration configuration, ICharacterMonitor characterMonitor, IMarketCache marketCache) : base(logger, imGuiService)
        {
            _configuration = configuration;
            _characterMonitor = characterMonitor;
            _marketCache = marketCache;
            Name = "市场布告板 " + configuration.MarketSaleHistoryLimit + " 成交计数";
            HelpText = "显示过去 " + configuration.MarketSaleHistoryLimit +
                       " 天内的成交数量。";
            ShowOperatorTooltip = true;
        }

        public override string Key { get; set; } = "MBSaleCount";
        public override string Name { get; set; } = "市场布告板成交计数器";

        public override string HelpText { get; set; } = "显示 X 天内的成交数量。";

        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Market;



        public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
        {
            return FilterItem(configuration, item.Item);
        }

        public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
        {
            var currentValue = CurrentValue(configuration);
            if (HasValueSet(configuration))
            {
                if (!item.CanBeTraded)
                {
                    return false;
                }
                var activeCharacter = _characterMonitor.ActiveCharacter;
                if (activeCharacter != null)
                {
                    var marketBoardData = _marketCache.GetPricing(item.RowId, activeCharacter.WorldId, false);
                    if (marketBoardData != null)
                    {
                        return marketBoardData.SevenDaySellCount.PassesFilter(currentValue.ToLower());
                    }
                }

                return false;
            }

            return null;
        }
    }
}