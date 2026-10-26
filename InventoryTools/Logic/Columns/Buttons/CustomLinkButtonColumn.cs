using System.Collections.Generic;
using AllaganLib.Shared.Extensions;
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

public class CustomLinkButtonColumn : ButtonColumn
{
    private readonly ICommandManager _commandManager;
    private readonly StringColumnSetting _actionSetting;
    private readonly StringColumnSetting _buttonText;

    public CustomLinkButtonColumn(StringColumnSetting.Factory stringColumnFactory, ICommandManager commandManager)
    {
        _commandManager = commandManager;
        _actionSetting = stringColumnFactory.Invoke("cb_action", "Command",
            "要打开的网页链接。可添加 ***名称*** 输出物品名称，或 ***ID*** 在命令中输出物品 ID。",
            "", "https://site.com");
        _buttonText = stringColumnFactory.Invoke("cb_label", "Label", "给按钮的标签。","","Button");
        Settings.Add(_buttonText);
        Settings.Add(_actionSetting);
    }
    public override string Name { get; set; } = "自定义链接按钮";
    public override float Width { get; set; } = 50;

    public override bool HasFilter { get; set; } = false;

    public override string HelpText { get; set; } =
        "自定义按钮，可打开将物品名称或 ID 嵌入链接的网页。";

    public override List<MessageBase>? Draw(FilterConfiguration configuration, ColumnConfiguration columnConfiguration, SearchResult searchResult,
        int rowIndex, int columnIndex)
    {
        ImGui.TableNextColumn();
        if (ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled))
        {
            using var id = ImRaii.PushId(rowIndex + columnIndex.ToString());
            if (ImGui.Button(_buttonText.CurrentValue(columnConfiguration)))
            {
                var webUrl = _actionSetting.CurrentValue(columnConfiguration);
                if (webUrl != null)
                {
                    if (webUrl.Contains("***ID***"))
                    {
                        webUrl = webUrl.Replace("***ID***", searchResult.ItemId.ToString());
                    }

                    if (webUrl.Contains("***名称***"))
                    {
                        webUrl = webUrl.Replace("***名称***", searchResult.Item.NameString);
                    }
                    webUrl.OpenBrowser();
                }
            }
        }

        return null;
    }
}