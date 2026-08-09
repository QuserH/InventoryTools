using System;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class IsCollectableFilter : GenericBooleanFilter
{
    public IsCollectableFilter(ILogger<IsCollectableFilter> logger, ImGuiService imGuiService) : base("IsCollectable", LocalizationService.Ui("Is Collectable?"), LocalizationService.Ui("Is the item collectable?"), FilterCategory.Basic, item => item.Item.Base.IsCollectable, item => item.Base.IsCollectable, logger, imGuiService)
    {
    }
}