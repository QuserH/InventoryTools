using System.Collections.Generic;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Logic.Columns.Abstract.ColumnSettings;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns.Buttons;

public class CustomButtonColumn : ButtonColumn
{
    private readonly ICommandManager _commandManager;
    private readonly StringColumnSetting _actionSetting;
    private readonly StringColumnSetting _buttonText;

    public CustomButtonColumn(StringColumnSetting.Factory stringColumnFactory, ICommandManager commandManager)
    {
        _commandManager = commandManager;
        _actionSetting = stringColumnFactory.Invoke("cb_action", "Command",
            "要运行的命令。会自动为你添加斜杠。可添加 ***名称*** 输出物品名称，或 ***ID*** 在命令中输出物品 ID。",
            "", "采集 ***名称***");
        _buttonText = stringColumnFactory.Invoke("cb_label", "Label", "给按钮的标签。","","Button");
        Settings.Add(_buttonText);
        Settings.Add(_actionSetting);
    }
    public override string Name { get; set; } = "自定义按钮";
    public override float Width { get; set; } = 50;

    public override bool HasFilter { get; set; } = false;

    public override string HelpText { get; set; } =
        "自定义按钮，可指定要使用物品名称或 ID 运行的自定义命令";

    public override List<MessageBase>? Draw(FilterConfiguration configuration, ColumnConfiguration columnConfiguration, SearchResult searchResult,
        int rowIndex, int columnIndex)
    {
        ImGui.TableNextColumn();
        if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
        {
            using var id = ImRaii.PushId(rowIndex + columnIndex.ToString());
            if (ImGui.Button(_buttonText.CurrentValue(columnConfiguration)))
            {
                var command = _actionSetting.CurrentValue(columnConfiguration);
                if (command != null)
                {
                    if (command.Contains("***ID***"))
                    {
                        command = command.Replace("***ID***", searchResult.ItemId.ToString());
                    }

                    if (command.Contains("***名称***"))
                    {
                        command = command.Replace("***名称***", searchResult.Item.NameString);
                    }

                    _commandManager.ProcessCommand("/" + command);
                }
            }
        }

        return null;
    }
}