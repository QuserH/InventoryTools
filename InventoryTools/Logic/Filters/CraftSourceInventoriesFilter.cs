using System.Collections.Generic;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Editors;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftSourceInventoriesFilter : InventoryScopeFilter
{
    public CraftSourceInventoriesFilter(InventoryScopePicker scopePicker, ILogger<CraftSourceInventoriesFilter> logger, ImGuiService imGuiService) : base(scopePicker, logger, imGuiService)
    {
    }

    public override string Key { get; set; } = "CraftSourceInventories";
    public override string Name { get; set; } = "待取回背包";

    public override string HelpText { get; set; } =
        "制作列表应检查哪些背包中的素材？所选背包中的物品将出现在雇员/背包中的物品列表中，根据制作列表的配置，你需要在采集前或采集后取回它们。";

    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Inventories;

    public override List<InventorySearchScope>? DefaultValue { get; set; } = null;

    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    public override List<InventorySearchScope>? GenerateDefaultScope()
    {
        return new List<InventorySearchScope>()
        {
            new InventorySearchScope() { ActiveCharacter = true, Categories = [InventoryCategory.RetainerBags, InventoryCategory.FreeCompanyBags, InventoryCategory.CharacterSaddleBags, InventoryCategory.CharacterPremiumSaddleBags] }
        };
    }

    public override int Order { get; set; } = -3;
}