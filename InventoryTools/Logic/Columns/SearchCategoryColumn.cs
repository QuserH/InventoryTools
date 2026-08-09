using InventoryTools.Localization;
﻿using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class SearchCategoryColumn : TextColumn
    {
        public SearchCategoryColumn(ILogger<SearchCategoryColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.Item.Base.ItemSearchCategory.IsValid)
            {
                return searchResult.Item.Base.ItemSearchCategory.Value.Name.ExtractText();
            }

            return "";
        }
        public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Category (Marketboard)"));
        public override string RenderName => LocalizationService.Ui(LocalizationService.Ui("MB Category"));
        public override float Width { get; set; } = 200.0f;

        public override string HelpText { get; set; } =
            LocalizationService.Ui(LocalizationService.Ui("The category of the item based off the market board search categories."));
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override FilterType DefaultIn => Logic.FilterType.GameItemFilter;
    }
}