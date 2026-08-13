using System.Collections.Generic;
using System.Linq;
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
        public override ColumnCategory ColumnCategory => ColumnCategory.Inventory;

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
            ImGui.TableNextColumn();
            if (!ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
                return null;

            // The retrieve action only applies to items actually held by a retainer; character
            // inventory rows should not offer it.
            if (searchResult.InventoryItem == null || searchResult.InventoryItem.RetainerId == 0)
                return null;

            var quantity = GetCraftListQuantity(configuration, searchResult);
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
                    new Dictionary<RetainerRetrievalItemKey, uint> { [key] = quantity }, true);
            }

            ImGuiUtil.HoverTooltip(LocalizationService.Ui("Retrieve this item's required quantity from retainers."));
            return null;
        }

        public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Amount to Retrieve"));
        public override string RenderName => LocalizationService.Ui("Retrieve");
        public override float Width { get; set; } = 110;
        public override string HelpText { get; set; } =
            LocalizationService.Ui(LocalizationService.Ui("This is the amount to retrieve from retainers."));
        public override FilterType AvailableIn { get; } = Logic.FilterType.CraftFilter;
        public override bool HasFilter { get; set; } = false;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override FilterType DefaultIn => Logic.FilterType.CraftFilter;
        public override bool? CraftOnly => false;

        private static uint GetCraftListQuantity(FilterConfiguration configuration, SearchResult searchResult)
        {
            // The inventory table contains InventoryItem results, not CraftItem results. Resolve
            // the corresponding craft requirement by item id and quality flags so the per-item
            // action is available next to the actual held stack.
            return configuration.CraftList.GetFlattenedMaterials()
                .Where(item => !item.IsOutputItem && item.ItemId == searchResult.ItemId &&
                               MatchesFlags(item.Flags, searchResult.Flags))
                .Aggregate(0u, (total, item) => total + item.QuantityWillRetrieve);
        }

        private static bool MatchesFlags(FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags required,
            FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags actual)
        {
            if (required.HasFlag(FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.Collectable))
                return actual.HasFlag(FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.Collectable);
            if (required.HasFlag(FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.HighQuality))
                return actual.HasFlag(FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.HighQuality);
            return !actual.HasFlag(FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.HighQuality) &&
                   !actual.HasFlag(FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.Collectable);
        }
    }
}
