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
    public class MarketBoardPriceFilter : StringFilter
    {
        protected readonly ICharacterMonitor CharacterMonitor;
        protected readonly IMarketCache MarketCache;

        public MarketBoardPriceFilter(ILogger<MarketBoardPriceFilter> logger, ImGuiService imGuiService, ICharacterMonitor characterMonitor, IMarketCache marketCache) : base(logger, imGuiService)
        {
            CharacterMonitor = characterMonitor;
            MarketCache = marketCache;
            ShowOperatorTooltip = true;
        }
        public override string Key { get; set; } = "MBPrice";
        public override string Name { get; set; } = "市场布告板平均价";
        public override string HelpText { get; set; } = "物品的市场布告板价格。使用此功能需要启用自动定价，另请注意，后台价格更新需在触发背包刷新事件后才会生效（该事件相当频繁）。";
        public override FilterCategory FilterCategory { get; set; } = FilterCategory.Market;

        public override bool? FilterItem(FilterConfiguration configuration,InventoryItem item)
        {
            var currentValue = CurrentValue(configuration);
            if (!string.IsNullOrEmpty(currentValue))
            {
                if (!item.CanBeTraded)
                {
                    return false;
                }
                var activeCharacter = CharacterMonitor.ActiveCharacter;
                if (activeCharacter != null)
                {
                    var marketBoardData = MarketCache.GetPricing(item.ItemId, activeCharacter.WorldId, false);
                    if (marketBoardData != null)
                    {
                        float price;
                        if (item.IsHQ)
                        {
                            price = marketBoardData.AveragePriceHq;
                        }
                        else
                        {
                            price = marketBoardData.AveragePriceNq;
                        }

                        return price.PassesFilter(currentValue.ToLower());
                    }
                }

                return false;
            }

            return true;
        }

        public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
        {
            var currentValue = CurrentValue(configuration);
            if (!string.IsNullOrEmpty(currentValue))
            {
                if (!item.CanBeTraded)
                {
                    return false;
                }
                var activeCharacter = CharacterMonitor.ActiveCharacter;
                if (activeCharacter != null)
                {
                    var marketBoardData = MarketCache.GetPricing(item.RowId, activeCharacter.WorldId, false);
                    if (marketBoardData != null)
                    {
                        float price = marketBoardData.AveragePriceNq;
                        return price.PassesFilter(currentValue.ToLower());
                    }
                }

                return false;
            }

            return true;
        }
    }
}