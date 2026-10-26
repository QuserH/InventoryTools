using System.Collections.Generic;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Editors;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;



public class ContextMenuItemSearchScopeSetting : Setting<List<InventorySearchScope>?>
{
    private readonly InventoryScopePicker _scopePicker;

    public ContextMenuItemSearchScopeSetting(ILogger<ContextMenuItemSearchScopeSetting> logger, ImGuiService imGuiService, InventoryScopePicker scopePicker) : base(logger, imGuiService)
    {
        _scopePicker = scopePicker;
    }
    public override List<InventorySearchScope>? DefaultValue { get; set; } = null;
    public override List<InventorySearchScope>? CurrentValue(InventoryToolsConfiguration configuration)
    {
        if (configuration.ItemSearchScope == null || configuration.ItemSearchScope.Count == 0)
        {
            return DefaultValue;
        }
        return configuration.ItemSearchScope;
    }

    public override void Draw(InventoryToolsConfiguration configuration, string? customName, bool? disableReset,
        bool? disableColouring)
    {
        var currentScopes = CurrentValue(configuration) ?? new List<InventorySearchScope>();
        if (disableColouring != true && HasValueSet(configuration))
        {
            ImGui.PushStyleColor(ImGuiCol.Text, ImGuiColors.HealerGreen);
            ImGui.LabelText("##" + Key + "标签", Name);
            ImGui.PopStyleColor();
        }
        else
        {
            ImGui.LabelText("##" + Key + "标签", Name);
        }

        ImGui.SetNextItemWidth(InputSize - 26);
        if (_scopePicker.Draw("##tooltipScope", currentScopes))
        {
            UpdateFilterConfiguration(configuration, currentScopes);
        }

        ImGui.SameLine();
        ImGuiService.HelpMarker(HelpText, Image, ImageSize);
        if (disableReset != true && HasValueSet(configuration))
        {
            ImGui.SameLine();
            if (ImGui.Button("重置##" + Key + "重置"))
            {
                Reset(configuration);
            }
        }
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, List<InventorySearchScope>? newValue)
    {
        configuration.ItemSearchScope = newValue;
    }

    public override string Key { get; set; } = "ItemSearchScope";
    public override string Name { get; set; } = "快捷菜单 - 搜索范围";
    public override string HelpText { get; set; } = "在亚拉戈工具箱已知的背包中搜索物品时，应搜索哪些背包？";

    public override string WizardName { get; } = "搜索范围";
    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ContextMenu;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override string Version => "1.7.0.13";
}