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
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class MarketBoardTotalPriceColumn : MarketBoardPriceColumn
    {
        public MarketBoardTotalPriceColumn(ILogger<MarketBoardTotalPriceColumn> logger, ImGuiService imGuiService, MarketboardWorldSetting marketboardWorldSetting, ICharacterMonitor characterMonitor, IMarketCache marketCache) : base(logger, imGuiService, marketboardWorldSetting, characterMonitor, marketCache)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Market;
        public override FilterType AvailableIn => Logic.FilterType.SearchFilter | Logic.FilterType.SortingFilter;

        public override List<MessageBase>? DoDraw(SearchResult searchResult, (int, int)? currentValue, int rowIndex,
            FilterConfiguration filterConfiguration, ColumnConfiguration columnConfiguration)
        {
            if (currentValue.HasValue && currentValue.Value.Item1 == Loading)
            {
                ImGui.TableNextColumn();
                if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
                {
                    ImGui.TextColored(ImGuiColors.DalamudYellow, LoadingString);
                }
            }
            else if (currentValue.HasValue && currentValue.Value.Item1 == Untradable)
            {
                ImGui.TableNextColumn();
                if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
                {
                    ImGui.TextColored(ImGuiColors.DalamudRed, UntradableString);
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
            if (searchResult.InventoryItem is {CanBeTraded: false})
            {
                return (Untradable, Untradable);
            }
            var value = base.CurrentValue(columnConfiguration, searchResult);
            var quantity = searchResult.InventoryItem?.Quantity ?? 1;
            return value.HasValue ? ((int)(value.Value.Item1 * quantity), (int)(value.Value.Item2 * quantity)) : null;
        }
        public override string Name { get; set; } = "市场布告板平均总价（数量×价格）普通/优质";
        public override string RenderName => "市场平均总价 普通/优质";
        public override string HelpText { get; set; } =
            "显示物品NQ与HQ两种形态的平均价，并乘以可用数量。未选择服务器时使用所属服务器。数据来源为 Universalis。";
        public override float Width { get; set; } = 250.0f;
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}