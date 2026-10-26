using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class StackSizeFilter : GenericIntegerFilter
{
    public StackSizeFilter(ILogger<StackSizeFilter> logger, ImGuiService imGuiService) : base("StackSize", "堆叠数量", "物品的最大堆叠数。", FilterCategory.Basic, item => (int?)item.Item.Base.StackSize, item => (int?)item.Base.StackSize, logger, imGuiService)
    {
    }
}
