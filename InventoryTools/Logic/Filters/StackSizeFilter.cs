using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class StackSizeFilter : GenericIntegerFilter
{
    public StackSizeFilter(ILogger<StackSizeFilter> logger, ImGuiService imGuiService) : base("StackSize", LocalizationService.Ui("Stack Size"), LocalizationService.Ui("The maximum stack size of the item."), FilterCategory.Basic, item => (int?)item.Item.Base.StackSize, item => (int?)item.Base.StackSize, logger, imGuiService)
    {
    }
}
