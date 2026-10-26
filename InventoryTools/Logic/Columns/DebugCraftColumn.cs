using System.Collections.Generic;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class DebugCraftColumn : TextColumn
    {
        public DebugCraftColumn(ILogger<DebugCraftColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Debug;
        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return "";
        }

        public override List<MessageBase>? Draw(FilterConfiguration configuration,
            ColumnConfiguration columnConfiguration,
            SearchResult searchResult, int rowIndex, int columnIndex)
        {
            if (searchResult.CraftItem == null) return null;

            ImGui.TableNextColumn();
            if (!ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled)) return null;
            ImGui.Text("所需：" +  searchResult.CraftItem.QuantityRequired);
            ImGui.Text("需要：" +  searchResult.CraftItem.QuantityNeeded);
            ImGui.Text("更新前需要：" +  searchResult.CraftItem.QuantityNeededPreUpdate);
            ImGui.Text("可用：" +  searchResult.CraftItem.QuantityAvailable);
            ImGui.Text("就绪：" +  searchResult.CraftItem.QuantityReady);
            ImGui.Text("可制作数量：" +  searchResult.CraftItem.QuantityCanCraft);
            ImGui.Text("将取回：" + searchResult.CraftItem.QuantityWillRetrieve);
            return null;
        }

        public override string Name { get; set; } = "调试 - 制作";
        public override float Width { get; set; } = 200;
        public override string HelpText { get; set; } = "显示制作调试信息";
        public override bool HasFilter { get; set; } = true;
        public override bool IsDebug { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}