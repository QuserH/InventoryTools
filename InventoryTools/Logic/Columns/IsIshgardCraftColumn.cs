using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class IsIshgardCraftColumn : CheckboxColumn
    {
        public IsIshgardCraftColumn(ILogger<IsIshgardCraftColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.HasUsesByType(ItemInfoType.SkybuilderHandIn);
        }
        public override string Name { get; set; } = "天穹街制作？";
        public override float Width { get; set; } = 100;
        public override string HelpText { get; set; } = "该物品是否为天穹街制作物品？";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}