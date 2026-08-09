using System.Collections.Generic;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns.Buttons;

public class CopyItemNameButtonColumn(IClipboardService clipboardService) : ButtonColumn
{
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Copy Item Name Button"));
    public override float Width { get; set; } = 80;
    public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Copies the item name to the clipboard."));

    public override List<MessageBase>? Draw(FilterConfiguration configuration, ColumnConfiguration columnConfiguration, SearchResult searchResult,
        int rowIndex, int columnIndex)
    {
        ImGui.TableNextColumn();
        if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
        {
            if (ImGui.Button(LocalizationService.Ui(LocalizationService.Ui("Copy Name##")) + rowIndex + "_" + columnIndex))
            {
                clipboardService.CopyToClipboard(searchResult.Item.NameString);
            }
        }

        return null;
    }
}