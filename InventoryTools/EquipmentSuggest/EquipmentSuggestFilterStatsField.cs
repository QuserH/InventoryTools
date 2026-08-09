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
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Filter Stats"));

    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("Should items be filtered on their stats, this means when a crafter is selected only items with crafting related stats show, etc"));
    public override string Version { get; set; } = "1.12.0.10";
}