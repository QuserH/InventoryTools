using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Logic.Columns.ColumnSettings;
using InventoryTools.Logic.Editors;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using OtterGui;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class CraftCalculatorColumn : IntegerColumn, IDisposable
{
    private readonly IInventoryMonitor _inventoryMonitor;
    private readonly ICharacterMonitor _characterMonitor;
    private readonly ScopePickerColumnSetting _scopePickerColumnSetting;
    private readonly InventoryScopeCalculator _scopeCalculator;
    private readonly CraftCalculator.Factory _craftCalculatorFactory;

    public CraftCalculatorColumn(ILogger<CraftCalculatorColumn> logger, ImGuiService imGuiService, IInventoryMonitor inventoryMonitor, ICharacterMonitor characterMonitor, ScopePickerColumnSetting scopePickerColumnSetting, InventoryScopeCalculator scopeCalculator, CraftCalculator.Factory craftCalculatorFactory) : base(logger, imGuiService)
    {
        _inventoryMonitor = inventoryMonitor;
        _characterMonitor = characterMonitor;
        _scopePickerColumnSetting = scopePickerColumnSetting;
        _scopeCalculator = scopeCalculator;
        _craftCalculatorFactory = craftCalculatorFactory;
    }
    public override ColumnCategory ColumnCategory => ColumnCategory.Tools;
    public Dictionary<uint, uint>? _craftable;
    public CraftCalculator? _craftCalculator;

    public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        if (_craftable == null) return 0;
        return (int?)(_craftable.ContainsKey(searchResult.Item.RowId) ? _craftable[searchResult.Item.RowId] : 0);
    }

    public override string Name { get; set; } = "制作计算器";
    public override float Width { get; set; } = 80;

    public override string HelpText { get; set; } =
        "根据你角色与雇员的物品计算可制作的总数量。可在下方选择自定义范围来覆盖检查的背包。";

    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    public override FilterType AvailableIn { get; } = Logic.FilterType.GameItemFilter;

    public override List<MessageBase>? DrawEditor(ColumnConfiguration columnConfiguration,
        FilterConfiguration configuration)
    {
        ImGui.NewLine();
        ImGui.Separator();
        ImGui.SetNextItemWidth(220);
        ImGui.LabelText("##" + configuration.Key + "搜索", "要搜索的背包：");
        ImGui.SetNextItemWidth(250);
        ImGui.SameLine();
        _scopePickerColumnSetting.Draw(columnConfiguration, "请确保至少包含一个装有水晶的背包，否则制作计算器将无法工作。若未选择范围，制作计算器将检查当前角色的背包及其雇员。");
        base.DrawEditor(columnConfiguration, configuration);
        return null;
    }


    public override IFilterEvent? DrawFooterFilter(ColumnConfiguration columnConfiguration, FilterTable filterTable)
    {
        ImGui.SameLine();

        if (_craftCalculator == null || !_craftCalculator.IsRunning)
        {
            if (ImGui.Button("计算制作"))
            {
                if (_craftCalculator == null)
                {
                    _craftCalculator = _craftCalculatorFactory.Invoke();
                    _craftCalculator.CraftingResult += CraftCalculatorOnCraftingResult;
                }

                var scopeSetting = _scopePickerColumnSetting.CurrentValue(columnConfiguration);

                if (scopeSetting == null)
                {
                    var items = new List<CriticalCommonLib.Models.InventoryItem>();
                    var playerBags = _inventoryMonitor.GetSpecificInventory(_characterMonitor.ActiveCharacterId,
                        InventoryCategory.CharacterBags);
                    var crystalBags = _inventoryMonitor.GetSpecificInventory(_characterMonitor.ActiveCharacterId,
                        InventoryCategory.Crystals);
                    var currencyBags = _inventoryMonitor.GetSpecificInventory(_characterMonitor.ActiveCharacterId,
                        InventoryCategory.Currency);
                    var inventories = _inventoryMonitor.Inventories;
                    foreach (var characterId in inventories)
                    {
                        var character = _characterMonitor.GetCharacterById(characterId.Key);
                        if (character != null)
                        {
                            if (character.OwnerId == _characterMonitor.ActiveCharacterId)
                            {
                                foreach (var inventoryCategory in characterId.Value.GetAllInventories())
                                {
                                    foreach (var inventory in inventoryCategory)
                                    {
                                        if (inventory != null)
                                        {
                                            items.Add(inventory);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    foreach (var item in playerBags)
                    {
                        items.Add(item);
                    }

                    foreach (var item in crystalBags)
                    {
                        items.Add(item);
                    }

                    foreach (var item in currencyBags)
                    {
                        items.Add(item);
                    }

                    _craftCalculator.SetAvailableItems(items);
                }
                else
                {
                    var itemList = _inventoryMonitor.AllItems.Where(c => _scopeCalculator.Filter(scopeSetting, c)).ToList();
                    _craftCalculator.SetAvailableItems(itemList);
                }

                foreach (var item in filterTable.RenderSearchResults)
                {
                    _craftCalculator.AddItemId(item.Item.RowId);
                }
                _craftCalculator.StartProcessing();
            }
        }
        else if (_craftCalculator.IsRunning)
        {
            if (ImGui.Button("停止计算制作"))
            {
                _craftCalculator.CancelProcessing();
            }
        }


        return null;
    }

    private void CraftCalculatorOnCraftingResult(object? sender, CraftingResultEventArgs e)
    {
        if (_craftable == null)
        {
            _craftable = new Dictionary<uint, uint>();
        }
        _craftable[e.ItemId] = e.CraftableQuantity ?? 0;
    }

    public override void Dispose()
    {
        if (!base.Disposed)
        {
            if (_craftCalculator != null)
            {
                _craftCalculator.CraftingResult -= CraftCalculatorOnCraftingResult;
                _craftCalculator.Dispose();
            }
            base.Dispose();
        }
    }
}