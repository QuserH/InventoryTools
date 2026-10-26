using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class IsArmoireItem : CheckboxColumn
    {
        public IsArmoireItem(ILogger<IsArmoireItem> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.CabinetCategory != null;
        }
        public override string Name { get; set; } = "收藏柜物品？";
        public override string RenderName => "收藏柜？";
        public override float Width { get; set; } = 100;
        public override string HelpText { get; set; } = "显示物品是否属于收藏柜。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
    }
}