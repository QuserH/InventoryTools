using System.Collections.Generic;
using CriticalCommonLib.MarketBoard;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Logic.Columns.ColumnSettings;
using InventoryTools.Services;
using InventoryTools.Ui.Widgets;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class MarketBoardMinTotalPriceColumn : MarketBoardMinPriceColumn
    {
        public MarketBoardMinTotalPriceColumn(ILogger<MarketBoardMinTotalPriceColumn> logger, ImGuiService imGuiService, MarketboardWorldSetting marketboardWorldSetting, ICharacterMonitor characterMonitor, IMarketCache marketCache) : base(logger, imGuiService, marketboardWorldSetting, characterMonitor, marketCache)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Market;
        public override string HelpText { get; set; } =
            "显示物品NQ与HQ两种形态的最低价，并乘以可用数量。未选择服务器时使用所属服务器。数据来源为 Universalis。";
        public override FilterType AvailableIn => Logic.FilterType.SearchFilter | Logic.FilterType.SortingFilter;

        public override List<MessageBase>? DoDraw(SearchResult searchResult, (int, int)? currentValue, int rowIndex,
            FilterConfiguration filterConfiguration, ColumnConfiguration columnConfiguration)
        {

            if (currentValue.HasValue && currentValue.Value.Item1 == Loading)
            {
                ImGui.TableNextColumn();
                if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
                {
                    ImGuiUtil.VerticalAlignTextColored(LoadingString, ImGuiColors.DalamudYellow,
                        filterConfiguration.TableHeight, false);
                }
            }
            else if (currentValue.HasValue && currentValue.Value.Item1 == Untradable)
            {
                ImGui.TableNextColumn();
                if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
                {
                    ImGuiUtil.VerticalAlignTextColored(UntradableString, ImGuiColors.DalamudRed,
                        filterConfiguration.TableHeight, false);
                }
            }
            else if(currentValue.HasValue)
            {
                base.DoDraw(searchResult, currentValue, rowIndex, filterConfiguration, columnConfiguration);
            }
            else
            {
                base.DoDraw(searchResult, currentValue, rowIndex, filterConfiguration, columnConfiguration);
            }

            return null;
        }

        public override (int, int)? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.CraftItem != null)
            {
                if (!searchResult.Item.CanBePlacedOnMarket)
                {
                    return (Untradable, Untradable);
                }
                var value = base.CurrentValue(columnConfiguration, searchResult);
                return value.HasValue ? ((int)(value.Value.Item1 * searchResult.CraftItem.QuantityRequired), (int)(value.Value.Item2 * searchResult.CraftItem.QuantityRequired)) : null;
            }
            if (searchResult.InventoryItem != null)
            {
                if (!searchResult.InventoryItem.CanBeTraded)
                {
                    return (Untradable, Untradable);
                }

                var value = base.CurrentValue(columnConfiguration, searchResult);
                return value.HasValue
                    ? ((int) (value.Value.Item1 * searchResult.InventoryItem.Quantity), (int) (value.Value.Item2 * searchResult.InventoryItem.Quantity))
                    : null;
            }

            return base.CurrentValue(columnConfiguration, searchResult);
        }

        public override string Name { get; set; } = "市场布告板最低总价（数量×价格）普通/优质";
        public override string RenderName => "市场最低总价 普通/优质";

        public override FilterType DefaultIn => Logic.FilterType.CraftFilter;
    }
}