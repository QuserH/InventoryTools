using System.Linq;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class RecipeUnlockedFilter : BooleanFilter
{
    private readonly IItemObtainabilityService _obtainabilityService;

    public RecipeUnlockedFilter(ILogger<RecipeUnlockedFilter> logger, ImGuiService imGuiService, IItemObtainabilityService obtainabilityService)
        : base(logger, imGuiService)
    {
        _obtainabilityService = obtainabilityService;
    }

    public override string Key { get; set; } = "RecipeUnlocked";
    public override string Name { get; set; } = "配方已解锁？";
    public override string HelpText { get; set; } = "按当前角色是否满足全部制作条件（职业等级、秘籍、专精）筛选物品。设置值后，无制作来源的物品将被排除。";
    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Crafting;
    public override FilterType AvailableIn { get; set; } = FilterType.GameItemFilter | FilterType.SearchFilter | FilterType.SortingFilter;

    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item) =>
        FilterItem(configuration, item.Item);

    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        var currentValue = CurrentValue(configuration);
        if (currentValue == null) return true;

        var recipe = item.Sources.OfType<ItemCraftResultSource>().FirstOrDefault()?.Recipe;
        if (recipe == null) return false;

        var requirements = _obtainabilityService.GetRequirements(item, IngredientPreferenceType.Crafting, recipe);
        if (requirements.Count == 0) return false;

        var allMet = requirements.All(r => r.IsMet);
        return currentValue.Value ? allMet : !allMet;
    }
}
