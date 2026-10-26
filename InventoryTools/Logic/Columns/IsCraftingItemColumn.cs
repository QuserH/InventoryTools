using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class IsCraftingItemColumn : CheckboxColumn
    {

        public IsCraftingItemColumn(ILogger<IsCraftingItemColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.HasUsesByType(ItemInfoType.CraftRecipe);
        }
        public override string Name { get; set; } = "制作材料？";
        public override string RenderName => "是否为制作素材？";
        public override float Width { get; set; } = 100;
        public override string HelpText { get; set; } = "该物品是否可用于制作其他物品？";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override FilterType DefaultIn => Logic.FilterType.GameItemFilter;
    }
}