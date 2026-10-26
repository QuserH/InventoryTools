using System.Collections.Generic;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Editors;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftDestinationInventoriesFilter : InventoryScopeFilter
{
    public CraftDestinationInventoriesFilter(InventoryScopePicker scopePicker, ILogger<CraftDestinationInventoriesFilter> logger, ImGuiService imGuiService) : base(scopePicker, logger, imGuiService)
    {
    }

    public override string Key { get; set; } = "CraftDestinationInventories";
    public override string Name { get; set; } = "取回至背包";

    public override string HelpText { get; set; } =
        "制作列表应将待取回背包中的物品整理到哪些背包？";

    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Inventories;

    public override List<InventorySearchScope>? DefaultValue { get; set; } = null;

    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    public override List<InventorySearchScope>? GenerateDefaultScope()
    {
        return new List<InventorySearchScope>()
        {
            new InventorySearchScope() { ActiveCharacter = true, Categories = [InventoryCategory.CharacterBags] }
        };
    }

    public override int Order { get; set; } = -2;
}