using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class IsAquariumItemColumn : CheckboxColumn
    {
        public IsAquariumItemColumn(ILogger<IsAquariumItemColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.HasUsesByType(ItemInfoType.Aquarium);
        }
        public override string Name { get; set; } = "水族箱物品？";
        public override float Width { get; set; } = 100;
        public override string HelpText { get; set; } = "该物品是否可放入水族箱？";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Choice;
    }
}