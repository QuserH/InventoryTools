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
                    return LocalizationService.Ui("On the Market");
                }

                if (searchResult.MergedContainsFreeCompany)
                {
                    return LocalizationService.Ui("Free Company Chest");
                }

                // Merged rows drop the specific slot but keep the container name, e.g.
                // "Saddlebag Left" instead of "Saddlebag Left - 5" or the generic "背包".
                if (searchResult.InventoryItem != null)
                {
                    return _itemLocalizer.SortedContainerName(searchResult.InventoryItem);
                }

                return LocalizationService.Ui("Merged Bag");
            }

            return null;
        }
        public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Inventory Location"));
        public override string RenderName => LocalizationService.Ui("Location");
        public override float Width { get; set; } = 100.0f;
        public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Shows the location of the item in your inventory."));
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override FilterType DefaultIn => Logic.FilterType.SearchFilter | Logic.FilterType.SortingFilter | Logic.FilterType.CraftFilter | Logic.FilterType.HistoryFilter;
    }
}
