using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class DebugColumn : TextColumn
    {
        public DebugColumn(ILogger<DebugColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Debug;

        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return "物品搜索：" + searchResult.Item.Base.ItemSearchCategory.RowId + " - 界面分类：" + searchResult.Item.Base.ItemUICategory.RowId + " - 排序分类：" + searchResult.Item.Base.ItemSortCategory.RowId + " - 装备栏位分类：" + searchResult.Item.Base.EquipSlotCategory.RowId + " - 职业分类：" + searchResult.Item.Base.ClassJobCategory.RowId + " - 购买：" + searchResult.Item.Base.PriceMid;
        }
        public override string Name { get; set; } = "调试 - 常规信息";
        public override float Width { get; set; } = 200;
        public override string HelpText { get; set; } = "显示基础调试信息";
        public override bool HasFilter { get; set; } = true;
        public override bool IsDebug { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}