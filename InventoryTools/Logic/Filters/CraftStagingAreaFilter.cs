using System.Collections.Generic;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Editors;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftStagingAreaFilter : InventoryScopeFilter
{
    public CraftStagingAreaFilter(InventoryScopePicker scopePicker, ILogger<CraftStagingAreaFilter> logger, ImGuiService imGuiService) : base(scopePicker, logger, imGuiService)
    {
    }

    public override string Key { get; set; } = "CraftStagingArea";
    public override string Name { get; set; } = "暂存区域";

    public override string HelpText { get; set; } =
        "制作时哪些背包应视为暂存区域？暂存区域中的物品视为用户背包的一部分。默认当前角色的背包、水晶和货币为暂存区域，但你也可以将陆行鸟鞍囊包含在内。";

    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Inventories;

    public override List<InventorySearchScope>? DefaultValue { get; set; } = null;

    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    public override List<InventorySearchScope>? GenerateDefaultScope()
    {
        return new List<InventorySearchScope>()
        {
            new InventorySearchScope() { ActiveCharacter = true, CharacterTypes = [CharacterType.Character], Categories = [InventoryCategory.CharacterBags, InventoryCategory.Currency, InventoryCategory.Crystals] }
        };
    }

    public override int Order { get; set; } = -1;
}