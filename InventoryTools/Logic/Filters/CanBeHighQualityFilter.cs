using System;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CanBeHighQualityFilter : GenericBooleanFilter
{
    public CanBeHighQualityFilter(ILogger<CanBeHighQualityFilter> logger, ImGuiService imGuiService) : base("CanBeHq", "可制成HQ？", "该物品是否可为HQ？", FilterCategory.Basic, item => item.Item.Base.CanBeHq, item => item.Base.CanBeHq, logger, imGuiService)
    {
    }
}