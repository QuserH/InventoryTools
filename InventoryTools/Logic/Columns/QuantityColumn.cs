using InventoryTools.Localization;
﻿using System.Collections.Generic;
using System.Linq;
using CharacterTools.Logic.Editors;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using FFXIVClientStructs.FFXIV.Client.Game;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Logic.Columns.ColumnSettings;
using InventoryTools.Services;
using InventoryTools.Ui.Widgets;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class QuantityColumn : IntegerColumn
    {
        private readonly IInventoryMonitor _inventoryMonitor;
        private readonly CharacterScopePickerColumnSetting _scopePickerColumnSetting;
        private readonly CharacterScopeCalculator _scopeCalculator;
        private readonly QualitySelectorSetting _qualitySelectorSetting;

        public QuantityColumn(ILogger<QuantityColumn> logger, ImGuiService imGuiService, IInventoryMonitor inventoryMonitor, CharacterScopePickerColumnSetting scopePickerColumnSetting, CharacterScopeCalculator scopeCalculator, QualitySelectorSetting qualitySelectorSetting) : base(logger, imGuiService)
        {
            _inventoryMonitor = inventoryMonitor;
            _scopePickerColumnSetting = scopePickerColumnSetting;
            _scopeCalculator = scopeCalculator;
            _qualitySelectorSetting = qualitySelectorSetting;
            this.Settings.Add(qualitySelectorSetting);
            this.FilterSettings.Add(qualitySelectorSetting);
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Inventory;
        public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.CuratedItem != null)
            {
                return (int)searchResult.CuratedItem.Quantity;
            }
            if (searchResult.InventoryItem != null)
            {
                return (int)searchResult.InventoryItem.Quantity;
            }

            var qualitySetting = _qualitySelectorSetting.CurrentValue(columnConfiguration)?.Select(c => c.Item1).ToList() ?? null;

            var scopes = _scopePickerColumnSetting.CurrentValue(columnConfiguration);
            if (scopes != null)
            {
                if (qualitySetting != null)
                {
                    return (int)qualitySetting.Sum(c => _scopeCalculator.Count(scopes, searchResult.ItemId, c));
                }
                else
                {
                    return (int)_scopeCalculator.Count(scopes, searchResult.ItemId) + (int)_scopeCalculator.Count(scopes, searchResult.ItemId, InventoryItem.ItemFlags.HighQuality);
                }
            }
            else
            {
                var qty = 0;
                if ((qualitySetting == null || qualitySetting.Contains(InventoryItem.ItemFlags.None)) && _inventoryMonitor.ItemCounts.ContainsKey((searchResult.Item.RowId,
                        FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.None)))
                {
                    qty += _inventoryMonitor.ItemCounts[(searchResult.Item.RowId,
                        FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.None)];
                }

                if ((qualitySetting == null || qualitySetting.Contains(InventoryItem.ItemFlags.HighQuality)) && _inventoryMonitor.ItemCounts.ContainsKey((searchResult.Item.RowId,
                        FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.HighQuality)))
                {
                    qty += _inventoryMonitor.ItemCounts[(searchResult.Item.RowId,
                        FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.HighQuality)];
                }

                if ((qualitySetting == null || qualitySetting.Contains(InventoryItem.ItemFlags.Collectable)) && _inventoryMonitor.ItemCounts.ContainsKey((searchResult.Item.RowId,
                        FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.Collectable)))
                {
                    qty += _inventoryMonitor.ItemCounts[(searchResult.Item.RowId,
                        FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.Collectable)];
                }

                return qty;
            }
        }

        public override List<MessageBase>? DrawEditor(ColumnConfiguration columnConfiguration,
            FilterConfiguration configuration)
        {
            if (configuration.FilterType == Logic.FilterType.GameItemFilter ||
                configuration.FilterType == Logic.FilterType.CraftFilter)
            {
                ImGui.NewLine();
                ImGui.Separator();
                ImGui.SetNextItemWidth(220);
                ImGui.LabelText("##" + configuration.Key + "搜索", "要搜索的角色：");
                ImGui.SetNextItemWidth(250);
                ImGui.SameLine();
                _scopePickerColumnSetting.Draw(columnConfiguration,
                    "用于设置要基于哪些角色生成总计。");
                base.DrawEditor(columnConfiguration, configuration);
            }
            return null;
        }

        public override List<MessageBase>? Draw(FilterConfiguration configuration, ColumnConfiguration columnConfiguration, SearchResult searchResult,
            int rowIndex, int columnIndex)
        {
            if (searchResult.CuratedItem != null)
            {
                ImGui.TableNextColumn();
                if (!ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled)) return null;
                var value = CurrentValue(columnConfiguration, searchResult)?.ToString() ?? "";
                ImGuiUtil.VerticalAlignButton(configuration.TableHeight);
                if (ImGui.InputText("##"+rowIndex+"QtyInput" + columnIndex, ref value, 4, ImGuiInputTextFlags.CharsDecimal))
                {
                    if (value != (CurrentValue(columnConfiguration, searchResult)?.ToString() ?? ""))
                    {
                        int parsedNumber;
                        if (int.TryParse(value, out parsedNumber))
                        {
                            if (parsedNumber < 0)
                            {
                                parsedNumber = 0;
                            }
                            var number = (uint)parsedNumber;
                            if (number != searchResult.CuratedItem.Quantity)
                            {
                                searchResult.CuratedItem.Quantity = number;
                                configuration.ConfigurationDirty = true;
                            }
                        }
                    }
                }

                return null;
            }
            return base.Draw(configuration, columnConfiguration, searchResult, rowIndex, columnIndex);
        }

        public override string Name { get; set; } = "数量/总可用数量";
        public override string RenderName => "数量";

        public override float Width { get; set; } = 70.0f;

        public override string HelpText { get; set; } =
            "物品数量。若从游戏物品或制作筛选查看，将显示所有背包中的总可用数量。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;

        public override FilterType AvailableIn => Logic.FilterType.SearchFilter | Logic.FilterType.SortingFilter | Logic.FilterType.GameItemFilter | Logic.FilterType.HistoryFilter | Logic.FilterType.CuratedList;
        public override FilterType DefaultIn => Logic.FilterType.SearchFilter | Logic.FilterType.SortingFilter | Logic.FilterType.CraftFilter | Logic.FilterType.HistoryFilter | Logic.FilterType.CuratedList;
    }
}