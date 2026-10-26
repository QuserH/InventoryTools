using AllaganLib.GameSheets.Caches;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class LeveIsCraftLevelColumn : CheckboxColumn
    {

        public LeveIsCraftLevelColumn(ILogger<LeveIsCraftLevelColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }

        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
        public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.HasSourcesByType(ItemInfoType.CraftLeve);
        }
        public override string Name { get; set; } = "是否为制作理符物品？";
        public override string RenderName => "制作理符";
        public override float Width { get; set; } = 100.0f;
        public override string HelpText { get; set; } = "该物品用于制作理符？";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
    }
}