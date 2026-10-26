using System.Collections.Generic;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns.Buttons;

public class CraftButtonColumn : ButtonColumn
{
    private readonly IGameInterface _gameInterface;
    private readonly IChatUtilities _chatUtilities;

    public CraftButtonColumn(IGameInterface gameInterface, IChatUtilities chatUtilities)
    {
        _gameInterface = gameInterface;
        _chatUtilities = chatUtilities;
    }
    public override string Name { get; set; } = "制作按钮";
    public override float Width { get; set; } = 80;
    public override string HelpText { get; set; } = "打开该物品制作笔记的按钮";

    public override List<MessageBase>? Draw(FilterConfiguration configuration, ColumnConfiguration columnConfiguration,
        SearchResult searchResult, int rowIndex, int columnIndex)
    {
        ImGui.TableNextColumn();
        if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
        {
            if (searchResult.Item.CanBeCrafted && ImGui.Button("制作##" + rowIndex + "_" + columnIndex))
            {
                var result = _gameInterface.OpenCraftingLog(searchResult.Item.RowId);
                if (!result)
                {
                    _chatUtilities.PrintError("无法打开制作笔记，你正在进行制作。");
                }
            }
        }

        return null;
    }
}