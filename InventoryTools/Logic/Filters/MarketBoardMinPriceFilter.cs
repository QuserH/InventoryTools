using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib.MarketBoard;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using InventoryTools.Localization;

using InventoryTools.Extensions;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters
{
    public class MarketBoardMinPriceFilter : MarketBoardPriceFilter
    {

        public MarketBoardMinPriceFilter(ILogger<MarketBoardMinPriceFilter> logger, ImGuiService imGuiService, ICharacterMonitor characterMonitor, IMarketCache marketCache) : base(logger, imGuiService, characterMonitor, marketCache)
        {
            ShowOperatorTooltip = true;
        }
        public override string Key { get; set; } = "MBMinPrice";
        public override string Name { get; set; } = "市场布告板最低价";
        public override string HelpText { get; set; } = "物品的市场布告板最低价。使用此功能需要启用自动定价，另请注意，后台价格更新需在触发背包刷新事件后才会生效（该事件相当频繁）。";
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
                            price = marketBoardData.MinPriceHq;
                        }
                        else
                        {
                            price = marketBoardData.MinPriceNq;
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
                        float price = marketBoardData.MinPriceNq;
                        return price.PassesFilter(currentValue.ToLower());
                    }
                }

                return false;
            }

            return true;
        }
    }
}