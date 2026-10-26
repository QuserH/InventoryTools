using System;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class GlamourReadyCombinedFilter : GenericBooleanFilter
{
    public GlamourReadyCombinedFilter(ILogger<GlamourReadyCombinedFilter> logger, ImGuiService imGuiService) : base("grCombined", "套装幻化组合",
        "该物品是否已在幻化柜中组合？", FilterCategory.Basic,
        item => item.SortedCategory == InventoryCategory.GlamourChest && item.GlamourId != 0, null, logger, imGuiService)
    {
    }
}