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
            return LocalizationService.Ui("Item Search: ") + searchResult.Item.Base.ItemSearchCategory.RowId + LocalizationService.Ui(" - Ui Category: ") + searchResult.Item.Base.ItemUICategory.RowId + LocalizationService.Ui(" - Sort Category: ") + searchResult.Item.Base.ItemSortCategory.RowId + LocalizationService.Ui(" - Equip Slot Category: ") + searchResult.Item.Base.EquipSlotCategory.RowId + LocalizationService.Ui(" - Class Job Category: ") + searchResult.Item.Base.ClassJobCategory.RowId + LocalizationService.Ui(" - Buy: ") + searchResult.Item.Base.PriceMid;
        }
        public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Debug - General Information"));
        public override float Width { get; set; } = 200;
        public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Shows basic debug information"));
        public override bool HasFilter { get; set; } = true;
        public override bool IsDebug { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}