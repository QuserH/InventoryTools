using InventoryTools.Localization;
﻿using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class UiCategoryColumn : TextColumn
    {
        public UiCategoryColumn(ILogger<UiCategoryColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            var itemItemUiCategory = searchResult.Item.Base.ItemUICategory.ValueNullable;
            if (itemItemUiCategory == null)
            {
                return null;
            }

            return itemItemUiCategory?.Name.ExtractText() ?? "";
        }

        public override string Name { get; set; } = "分类（基本）";
        public override string RenderName => "分类";
        public override float Width { get; set; } = 200.0f;
        public override string HelpText { get; set; } = "物品的分类。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override FilterType DefaultIn => Logic.FilterType.GameItemFilter;
    }
}