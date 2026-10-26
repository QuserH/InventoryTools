using System.Collections.Generic;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Extensions;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class ShortcutColumn : TextColumn
    {
        private readonly TryOn _tryOn;

        public ShortcutColumn(ILogger<ShortcutColumn> logger, ImGuiService imGuiService, TryOn tryOn) : base(logger, imGuiService)
        {
            _tryOn = tryOn;
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Tools;
        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return null;
        }

        public override string Name { get; set; } = "快捷操作";
        public override float Width { get; set; } = 32.0f;

        public override string HelpText { get; set; } =
            "提供一系列小按钮，用于打开 Garland Tools 和试穿物品。";
        public override bool HasFilter { get; set; } = false;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;

        public override List<MessageBase>? Draw(FilterConfiguration configuration,
            ColumnConfiguration columnConfiguration,
            SearchResult searchResult, int rowIndex, int columnIndex)
        {
            ImGui.TableNextColumn();
            if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
            {
                if (ImGui.SmallButton("大于##GT" + rowIndex))
                {
                    $"https://www.garlandtools.org/db/#item/{searchResult.Item.GarlandToolsId}".OpenBrowser();
                }

                if (searchResult.Item.CanTryOn)
                {
                    ImGui.SameLine();
                    if (ImGui.SmallButton("试穿##TO" + rowIndex))
                    {
                        if (_tryOn.CanUseTryOn)
                        {
                            _tryOn.TryOnItem(searchResult.Item, searchResult.InventoryItem?.Stain ?? 0,
                                searchResult.InventoryItem?.IsHQ ?? false);
                        }
                        else
                        {
                            Logger.LogError("尝试试穿时出现问题：" +
                                            searchResult.Item.NameString);
                        }
                    }
                }
            }

            return null;
        }
    }
}