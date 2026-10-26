using System;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class IsUnobtainableFilter : GenericBooleanFilter
{
    public IsUnobtainableFilter(ILogger<IsUnobtainableFilter> logger, ImGuiService imGuiService) : base("IsUnobtainable", "该物品是否已绝版？", "该物品已绝版？", FilterCategory.Basic, item => item.Item.IsUnobtainable, item => item.IsUnobtainable,  logger, imGuiService)
    {
    }
}