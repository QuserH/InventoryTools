using System.Collections.Generic;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using InventoryTools.Ui.Widgets;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class CraftAmountCanCraftColumn : IntegerColumn
    {
        public CraftAmountCanCraftColumn(ILogger<CraftAmountCanCraftColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Crafting;
        public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.CraftItem == null) return 0;
            return (int?) (searchResult.CraftItem.CraftOperationsRequired);
        }

        public override List<MessageBase>? Draw(FilterConfiguration configuration,
            ColumnConfiguration columnConfiguration,
            SearchResult searchResult, int rowIndex, int columnIndex)
        {
            if (searchResult.CraftItem == null) return null;

            ImGui.TableNextColumn();
            if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
            {
                if (CurrentValue(columnConfiguration, searchResult) > 0)
                {
                    ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.ParsedBlue);
                }

                var currentValue = CurrentValue(columnConfiguration, searchResult);
                if (currentValue != null)
                {
                    var fmt = $"{currentValue.Value:n0}";
                    if (searchResult.CraftItem.Yield > 1)
                    {
                        fmt += " (" + searchResult.CraftItem.Yield + ")";
                    }

                    ImGuiUtil.VerticalAlignText(fmt, configuration.TableHeight, false);
                }
                else
                {
                    ImGuiUtil.VerticalAlignText(EmptyText, configuration.TableHeight, false);
                }

                if (CurrentValue(columnConfiguration, searchResult) > 0)
                {
                    ImGui.PopStyleColor();
                }
            }

            return null;
        }

        public override string Name { get; set; } = "可制作数量";
        public override string RenderName => "可制作";
        public override float Width { get; set; } = 60;
        public override bool? CraftOnly => true;
        public override string HelpText { get; set; } =
            "根据你背包中的物品可以制作出的数量";
        public override bool HasFilter { get; set; } = false;
        public override FilterType AvailableIn { get; } = Logic.FilterType.CraftFilter;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}