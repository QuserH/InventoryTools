using System.Collections.Generic;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;
using OtterGui;

namespace InventoryTools.Logic.Columns
{
    public class CraftAmountAvailableColumn : IntegerColumn
    {
        private readonly RetainerRetrievalAutomation _retainerRetrievalAutomation;

        public CraftAmountAvailableColumn(ILogger<CraftAmountAvailableColumn> logger, ImGuiService imGuiService,
            RetainerRetrievalAutomation retainerRetrievalAutomation) : base(logger, imGuiService)
        {
            _retainerRetrievalAutomation = retainerRetrievalAutomation;
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Crafting;

        public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.CraftItem != null)
            {
                if (searchResult.CraftItem.IsOutputItem)
                {
                    return 0;
                }

                return (int) searchResult.CraftItem.QuantityWillRetrieve;
            }

            if (searchResult.SortingResult != null)
            {
                return searchResult.SortingResult.Quantity;
            }

            return 0;
        }

        public override List<MessageBase>? Draw(FilterConfiguration configuration,
            ColumnConfiguration columnConfiguration,
            SearchResult searchResult, int rowIndex, int columnIndex)
        {
            var craftItem = searchResult.CraftItem;
            if (craftItem?.IsOutputItem ?? false)
            {
                ImGui.TableNextColumn();
                return null;
            }

            ImGui.TableNextColumn();
            if (!ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
                return null;

            var quantity = craftItem?.QuantityWillRetrieve ?? 0;
            if (quantity == 0)
                return null;

            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedBlue);
            ImGui.TextUnformatted(quantity.ToString("N0"));
            ImGui.PopStyleColor();
            ImGui.SameLine();

            if (!_retainerRetrievalAutomation.IsRunning &&
                ImGui.SmallButton(LocalizationService.Ui("Retrieve") + "##retainerRetrieve" + rowIndex + searchResult.ItemId))
            {
                var key = new RetainerRetrievalItemKey(searchResult.ItemId, searchResult.Flags);
                _retainerRetrievalAutomation.Start(configuration.CraftList,
                    new Dictionary<RetainerRetrievalItemKey, uint> { [key] = quantity });
            }

            ImGuiUtil.HoverTooltip(LocalizationService.Ui("Retrieve this item's required quantity from retainers."));
            return null;

            /*
            if (craftItem != null && craftItem.QuantityWillRetrieve != 0)
            {
                ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedBlue);
            }

            base.Draw(configuration, columnConfiguration, searchResult, rowIndex, columnIndex);

            if (craftItem != null &&craftItem.QuantityWillRetrieve != 0)
            {
                ImGui.PopStyleColor();
            }
            return null;
            */
        }

        public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Amount to Retrieve"));
        public override string RenderName => LocalizationService.Ui("Retrieve");
        public override float Width { get; set; } = 60;
        public override bool? CraftOnly => false;

        public override string HelpText { get; set; } =
            LocalizationService.Ui(LocalizationService.Ui("This is the amount to retrieve from retainers."));
        public override FilterType AvailableIn { get; } = Logic.FilterType.CraftFilter | Logic.FilterType.SortingFilter;
        public override bool HasFilter { get; set; } = false;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override FilterType DefaultIn => Logic.FilterType.CraftFilter;
    }
}
