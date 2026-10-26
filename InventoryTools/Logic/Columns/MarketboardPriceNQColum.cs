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
    public class MarketBoardPriceNQColumn : GilColumn
    {
        private readonly ICharacterMonitor _characterMonitor;
        private readonly IMarketCache _marketCache;

        public MarketBoardPriceNQColumn(ILogger<MarketBoardPriceNQColumn> logger, ImGuiService imGuiService, ICharacterMonitor characterMonitor, IMarketCache marketCache, MarketboardWorldSetting marketboardWorldSetting) : base(logger, imGuiService)
        {
            MarketboardWorldSetting = marketboardWorldSetting;
            _characterMonitor = characterMonitor;
            _marketCache = marketCache;
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Market;
        protected readonly string LoadingString = "正在加载……";
        protected readonly string UntradableString = "不可交易";
        protected readonly int Loading = -1;
        protected readonly int Untradable = -2;

        public MarketboardWorldSetting MarketboardWorldSetting { get; }

        public override bool IsConfigurable => true;

        public override List<MessageBase>? DrawEditor(ColumnConfiguration columnConfiguration,
            FilterConfiguration configuration)
        {
            ImGui.NewLine();
            ImGui.Separator();
            MarketboardWorldSetting.Draw(columnConfiguration, null);
            return null;
        }

        public override List<MessageBase>? Draw(FilterConfiguration configuration,
            ColumnConfiguration columnConfiguration,
            SearchResult searchResult, int rowIndex, int columnIndex)
        {
            return DoDraw(searchResult, CurrentValue(columnConfiguration, searchResult), rowIndex, configuration, columnConfiguration);
        }
        public override List<MessageBase>? DoDraw(SearchResult searchResult, int? currentValue, int rowIndex,
            FilterConfiguration filterConfiguration, ColumnConfiguration columnConfiguration)
        {
            if (currentValue.HasValue && currentValue.Value == Loading)
            {
                ImGui.TableNextColumn();
                if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
                {
                    ImGui.TextColored(ImGuiColors.DalamudYellow, LoadingString);
                }
            }
            else if (currentValue.HasValue && currentValue.Value == Untradable)
            {
                ImGui.TableNextColumn();
                if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
                {
                    ImGui.TextColored(ImGuiColors.DalamudRed, UntradableString);
                }
            }
            else
            {
                base.DoDraw(searchResult, currentValue, rowIndex, filterConfiguration, columnConfiguration);
            }
            return null;
        }
        public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.InventoryItem is {CanBeTraded: false})
            {
                return Untradable;
            }

            if (!searchResult.Item.CanBePlacedOnMarket)
            {
                return Untradable;
            }
            var activeCharacter = _characterMonitor.ActiveCharacter;
            if (activeCharacter != null)
            {
                var selectedWorldId = MarketboardWorldSetting.SelectedWorldId(columnConfiguration, activeCharacter);
                var marketBoardData = _marketCache.GetPricing(searchResult.Item.RowId, selectedWorldId, false);
                if (marketBoardData != null)
                {
                    var nq = marketBoardData.AveragePriceNq;
                    return (int)nq;
                }
            }

            return Loading;
        }
        public override string Name { get; set; } = "市场布告板普通品平均价";
        public override string RenderName => "市场平均价（NQ）";
        public override string HelpText { get; set; } =
            "显示物品NQ形态的平均价。未选择服务器时使用所属服务器。数据来源为 Universalis。";
        public override float Width { get; set; } = 250.0f;
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}
