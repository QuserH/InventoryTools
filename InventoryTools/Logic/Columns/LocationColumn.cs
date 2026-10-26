using InventoryTools.Localization;
﻿using InventoryTools.Localizers;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class LocationColumn : TextColumn
    {
        private readonly ItemLocalizer _itemLocalizer;

        public LocationColumn(ILogger<LocationColumn> logger, ImGuiService imGuiService, ItemLocalizer itemLocalizer) : base(logger, imGuiService)
        {
            _itemLocalizer = itemLocalizer;
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Inventory;

        public override FilterType AvailableIn => Logic.FilterType.SearchFilter | Logic.FilterType.SortingFilter | Logic.FilterType.CraftFilter;

        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            // Merged rows combine several stacks, so there is no single bag location to show.
            // Market listings are the exception: they are still reported as being on the market.
            if (searchResult.InventoryItem != null && !searchResult.IsMerged)
            {
                return _itemLocalizer.FormattedBagLocation(searchResult.InventoryItem);
            }

            if (searchResult.IsMerged)
            {
                if (searchResult.MergedContainsMarket)
                {
                    return "市场上";
                }

                if (searchResult.MergedContainsFreeCompany)
                {
                    return "部队储物柜";
                }

                // Merged rows drop the specific slot but keep the container name, e.g.
                // "Saddlebag Left" instead of "Saddlebag Left - 5" or the generic "背包".
                if (searchResult.InventoryItem != null)
                {
                    return _itemLocalizer.SortedContainerName(searchResult.InventoryItem);
                }

                return "背包";
            }

            return null;
        }
        public override string Name { get; set; } = "背包位置";
        public override string RenderName => "位置";
        public override float Width { get; set; } = 100.0f;
        public override string HelpText { get; set; } = "显示物品在你背包中的位置。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override FilterType DefaultIn => Logic.FilterType.SearchFilter | Logic.FilterType.SortingFilter | Logic.FilterType.CraftFilter | Logic.FilterType.HistoryFilter;
    }
}
