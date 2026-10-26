using InventoryTools.Localization;
﻿using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class EquippableColumn : TextColumn
    {
        public EquippableColumn(ILogger<EquippableColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.ClassJobCategory?.Base.Name.ExtractText() ?? "";
        }
        public override string Name { get; set; } = "可装备职业";
        public override float Width { get; set; } = 200;
        public override string HelpText { get; set; } = "显示物品可被哪些职业装备";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}