using InventoryTools.Localization;
﻿using System.Numerics;
using Dalamud.Interface.Colors;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class NameColumn : ColoredTextColumn
    {
        public NameColumn(ILogger<NameColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override (string, Vector4)? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            var name = searchResult.CraftItem?.FormattedName;
            if (name == null)
            {
                name = searchResult.IsMerged
                    ? searchResult.Item.NameString
                    : searchResult.InventoryItem?.FormattedName ?? searchResult.Item.NameString;
            }

            return (
                name,
                searchResult.InventoryItem?.ItemColour ?? ImGuiColors.DalamudWhite);
        }
        public override string Name { get; set; } = LocalizationService.Ui("Name");
        public override float Width { get; set; } = 250.0f;
        public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("The name of the item."));
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;

        public override FilterType DefaultIn => Logic.FilterType.SearchFilter | Logic.FilterType.SortingFilter | Logic.FilterType.GameItemFilter | Logic.FilterType.CraftFilter | Logic.FilterType.HistoryFilter;
    }
}