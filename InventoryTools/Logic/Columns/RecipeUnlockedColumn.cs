using System.Linq;
using AllaganLib.GameSheets.ItemSources;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class RecipeUnlockedColumn : CheckboxColumn
{
    private readonly IItemObtainabilityService _obtainabilityService;

    public RecipeUnlockedColumn(ILogger<RecipeUnlockedColumn> logger, ImGuiService imGuiService, IItemObtainabilityService obtainabilityService)
        : base(logger, imGuiService)
    {
        _obtainabilityService = obtainabilityService;
    }

    public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

    public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        var recipe = searchResult.Item.Sources.OfType<ItemCraftResultSource>().FirstOrDefault()?.Recipe;
        if (recipe == null) return null;

        var requirements = _obtainabilityService.GetRequirements(searchResult.Item, IngredientPreferenceType.Crafting, recipe);
        if (requirements.Count == 0) return null;

        return requirements.All(r => r.IsMet);
    }

    public override string Name { get; set; } = "配方已解锁？";
    public override string RenderName => "配方已解锁？";
    public override float Width { get; set; } = 140.0f;
    public override string HelpText { get; set; } = "显示当前角色是否满足制作该物品的全部条件（职业等级、秘籍、专精）。";
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
    public override FilterType DefaultIn => Logic.FilterType.GameItemFilter;
}
