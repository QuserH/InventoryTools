using InventoryTools.Localization;
﻿using System.Collections.Generic;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using InventoryTools.Extensions;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using InventoryTools.Ui.Widgets;
using Microsoft.Extensions.Logging;
using InventoryItem = FFXIVClientStructs.FFXIV.Client.Game.InventoryItem;

namespace InventoryTools.Logic.Columns
{
    public class TypeColumn : TextColumn
    {
        public TypeColumn(ILogger<TypeColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

        public override List<MessageBase>? Draw(FilterConfiguration configuration, ColumnConfiguration columnConfiguration, SearchResult searchResult,
            int rowIndex, int columnIndex)
        {
            if (searchResult.CuratedItem != null && (searchResult.Item.IsCollectable || searchResult.Item.Base.CanBeHq))
            {
                ImGui.TableNextColumn();
                if (!ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled)) return null;
                var value = searchResult.CuratedItem.ItemFlags.FormattedName();
                ImGuiUtil.VerticalAlignButton(configuration.TableHeight);
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                using (var combo = ImRaii.Combo("##"+rowIndex+"Type", value))
                {
                    if (combo)
                    {
                        if (ImGui.Selectable(InventoryItem.ItemFlags.None.FormattedName()))
                        {
                            searchResult.CuratedItem.ItemFlags = InventoryItem.ItemFlags.None;
                        }
                        if (ImGui.Selectable(InventoryItem.ItemFlags.HighQuality.FormattedName()))
                        {
                            searchResult.CuratedItem.ItemFlags = InventoryItem.ItemFlags.HighQuality;
                        }
                        if (ImGui.Selectable(InventoryItem.ItemFlags.Collectable.FormattedName()))
                        {
                            searchResult.CuratedItem.ItemFlags = InventoryItem.ItemFlags.Collectable;
                        }

                        configuration.ConfigurationDirty = true;
                    }
                }

                return null;
            }
            return base.Draw(configuration, columnConfiguration, searchResult, rowIndex, columnIndex);
        }

        /// <summary>显示改为品质图标：HQ 显示 E03c，收藏品显示 E03d，NQ 不显示。筛选/排序/导出仍用文本值（HQ/NQ/Collectible）。</summary>
        public override List<MessageBase>? DoDraw(SearchResult searchResult, string? currentValue, int rowIndex,
            FilterConfiguration filterConfiguration, ColumnConfiguration columnConfiguration)
        {
            ImGui.TableNextColumn();
            if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
            {
                var display = currentValue switch
                {
                    "HQ" => "\uE03c",
                    "Collectible" => "\uE03d",
                    _ => ""
                };
                ImGuiUtil.VerticalAlignText(display, filterConfiguration.TableHeight, filterConfiguration.FilterType == Logic.FilterType.CraftFilter);
            }

            return null;
        }

        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.InventoryItem != null)
            {
                return searchResult.InventoryItem.FormattedType;
            }

            return null;
        }
        public override string Name { get; set; } = LocalizationService.Ui("Type");
        public override float Width { get; set; } = 80.0f;
        public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("The type of the item."));
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;

        public override FilterType DefaultIn => Logic.FilterType.SearchFilter | Logic.FilterType.SortingFilter | Logic.FilterType.HistoryFilter;
    }
}