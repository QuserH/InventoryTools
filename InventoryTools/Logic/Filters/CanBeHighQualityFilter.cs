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
    public CanBeHighQualityFilter(ILogger<CanBeHighQualityFilter> logger, ImGuiService imGuiService) : base("CanBeHq", LocalizationService.Ui("Can be High Quality?"), LocalizationService.Ui("Can the item be high quality?"), FilterCategory.Basic, item => item.Item.Base.CanBeHq, item => item.Base.CanBeHq, logger, imGuiService)
    {
    }
}