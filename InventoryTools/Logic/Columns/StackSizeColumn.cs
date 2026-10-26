using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class StackSizeColumn : IntegerColumn
    {
        public StackSizeColumn(ILogger<StackSizeColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }

        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

        public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return (int)searchResult.Item.Base.StackSize;
        }

        public override string Name { get; set; } = "堆叠数量";
        public override float Width { get; set; } = 80.0f;
        public override string HelpText { get; set; } = "物品的最大堆叠数。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}
