using System.Linq;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Enums;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using InventoryTools.Localization;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public sealed class CraftRetainerAvailableFilter : BooleanFilter
{
    private readonly IInventoryMonitor _inventoryMonitor;
    private readonly ICharacterMonitor _characterMonitor;

    public CraftRetainerAvailableFilter(ILogger<CraftRetainerAvailableFilter> logger, ImGuiService imGuiService,
        IInventoryMonitor inventoryMonitor, ICharacterMonitor characterMonitor) : base(logger, imGuiService)
    {
        _inventoryMonitor = inventoryMonitor;
        _characterMonitor = characterMonitor;
    }

    public override string Key { get; set; } = "CraftRetainerAvailable";
    public override string Name { get; set; } = LocalizationService.Ui("Retainer Material Available");
    public override string HelpText { get; set; } = LocalizationService.Ui("Show craft items found in one of the active character's retainers.");
    public override FilterCategory FilterCategory { get; set; } = FilterCategory.IngredientSourcing;
    public override int Order { get; set; } = 100;
    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    public override bool? DefaultValue { get; set; } = null;

    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return IsAvailable(item.ItemId);
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        return IsAvailable(item.RowId);
    }

    private bool IsAvailable(uint itemId)
    {
        if (!_characterMonitor.IsLoggedIn)
            return false;

        foreach (var retainer in _characterMonitor.GetRetainerCharacters(_characterMonitor.ActiveCharacterId))
        {
            if (!_inventoryMonitor.Inventories.TryGetValue(retainer.Key, out var inventory))
                continue;

            if (inventory.GetItemsByCategory(InventoryCategory.RetainerBags)
                .Any(item => item.ItemId == itemId && item.Quantity > 0))
                return true;
        }

        return false;
    }
}
