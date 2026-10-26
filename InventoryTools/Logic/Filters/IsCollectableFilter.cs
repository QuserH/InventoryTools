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
    public IsCollectableFilter(ILogger<IsCollectableFilter> logger, ImGuiService imGuiService) : base("IsCollectable", "是否为收藏品？", "该物品为收藏品？", FilterCategory.Basic, item => item.Item.Base.IsCollectable, item => item.Base.IsCollectable, logger, imGuiService)
    {
    }
}