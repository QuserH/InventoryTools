using AllaganLib.GameSheets.Model;
using AllaganLib.Interface.Grid.ColumnFilters;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Humanizer;
using Dalamud.Bindings.ImGui;
using InventoryTools.Services;
using InventoryTools.Localization;

namespace InventoryTools.EquipmentSuggest;

public class EquipmentSuggestSlotColumn : AllaganLib.Interface.Grid.StringColumn<EquipmentSuggestConfig,
    EquipmentSuggestItem, MessageBase>
{
    private readonly EquipmentSuggestModeSetting _modeSetting;
    private readonly InventoryToolsConfiguration _configuration;

    public EquipmentSuggestSlotColumn(ImGuiService imGuiService, StringColumnFilter stringColumnFilter, EquipmentSuggestModeSetting modeSetting, InventoryToolsConfiguration configuration) : base(imGuiService, stringColumnFilter)
    {
        _modeSetting = modeSetting;
        _configuration = configuration;
    }

    public override string DefaultValue { get; set; } = string.Empty;
    public override string Key { get; set; } = "Slot";

    public override string Name
    {
        get { return _modeSetting.CurrentValue(_configuration) == EquipmentSuggestMode.Class ? "栏位" : "职业"; }
        set { }
    }

    public override string HelpText { get; set; } = "要填充的栏位";
    public override string Version { get; set; } = "1.12.0.10";
    public override string? CurrentValue(EquipmentSuggestItem item)
    {
        if (item.EquipmentSlot != null)
        {
            switch (item.EquipmentSlot)
            {
                case EquipSlot.MainHand:
                    return "主手";
                case EquipSlot.OffHand:
                    return "副手";
                case EquipSlot.Head:
                    return "头部";
                case EquipSlot.Body:
                    return "身体";
                case EquipSlot.Gloves:
                    return "手部";
                case EquipSlot.Legs:
                    return "腿部";
                case EquipSlot.Feet:
                    return "脚部";
                case EquipSlot.Ears:
                    return "耳部";
                case EquipSlot.Neck:
                    return "颈部";
                case EquipSlot.Wrists:
                    return "腕部";
                case EquipSlot.FingerR:
                    return "右手戒指";
                case EquipSlot.FingerL:
                    return "左手戒指";
                case EquipSlot.SoulCrystal:
                    return "特职水晶";
            }

            return LocalizationService.Ui(item.EquipmentSlot.Value.Humanize());
        }

        if (item.ClassJobRow != null)
        {
            return item.ClassJobRow.Base.Name.ToImGuiString().Humanize();
        }

        return "未知";
    }

    public override string? RenderName { get; set; } = null;
    public override int Width { get; set; } = 100;
    public override bool HideFilter { get; set; } = true;
    public override ImGuiTableColumnFlags ColumnFlags { get; set; } = ImGuiTableColumnFlags.WidthFixed;
    public override string EmptyText { get; set; } = "";
}
