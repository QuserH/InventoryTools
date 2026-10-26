using AllaganLib.Interface.FormFields;
using AllaganLib.Interface.Services;
using ImGuiService = InventoryTools.Services.ImGuiService;
using InventoryTools.Localization;

namespace InventoryTools.EquipmentSuggest;

public class EquipmentSuggestFilterStatsField : BooleanFormField<EquipmentSuggestConfig>
{
    public EquipmentSuggestFilterStatsField(ImGuiService imGuiService) : base(imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = true;
    public override string Key { get; set; } = "FilterStats";
    public override string Name { get; set; } = "筛选状态";

    public override string HelpText { get; set; } =
        "是否按属性筛选物品。当选定工匠职业时，仅显示带有制作相关属性的物品。";
    public override string Version { get; set; } = "1.12.0.10";
}