using System.Collections.Generic;
using System.Linq;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class CraftRequiredQuantityColumn : IntegerColumn
    {
        public CraftRequiredQuantityColumn(ILogger<CraftRequiredQuantityColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Crafting;

        public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.CraftItem != null)
            {
                return (int)searchResult.CraftItem.QuantityRequired;
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

            var quantity = GetCraftListQuantity(configuration, searchResult);
            InventoryTools.Ui.Widgets.ImGuiUtil.VerticalAlignText(quantity.ToString("N0"), configuration.TableHeight, false);
            return null;
        }

        public override string Name { get; set; } = "需求数量";
        public override string RenderName => "所需";
        public override float Width { get; set; } = 60;
        public override string HelpText { get; set; } =
            "当前制作清单对该材料的总需求数量。";
        public override FilterType AvailableIn { get; } = Logic.FilterType.CraftFilter;
        public override bool HasFilter { get; set; } = false;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override FilterType DefaultIn => Logic.FilterType.CraftFilter;
        public override bool? CraftOnly => false;

        private CriticalCommonLib.Crafting.CraftList? _lastCraftList;
        private List<CriticalCommonLib.Crafting.CraftItem>? _lastMaterials;

        private uint GetCraftListQuantity(FilterConfiguration configuration, SearchResult searchResult)
        {
            var craftList = configuration.CraftList;
            if (!ReferenceEquals(_lastCraftList, craftList))
            {
                _lastCraftList = craftList;
                _lastMaterials = craftList.GetFlattenedMaterials();
            }

            return _lastMaterials!
                .Where(item => !item.IsOutputItem && item.ItemId == searchResult.ItemId &&
                               MatchesFlags(item.Flags, searchResult.Flags))
                .Aggregate(0u, (total, item) => total + item.QuantityRequired);
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
