using System;
using System.Collections.Generic;
using AllaganLib.Interface.FormFields;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.EquipmentSuggest;

public enum EquipmentSuggestToolModeCategory
{
    Crafting,
    Gathering,
    Combat,
    CombatTank,
    CombatHealer,
    CombatMelee,
    CombatRanged,
}

public class EquipmentSuggestToolModeCategorySetting : EnumFormField<EquipmentSuggestToolModeCategory, EquipmentSuggestConfig>
{
    public EquipmentSuggestToolModeCategorySetting(ImGuiService imGuiService) : base(imGuiService)
    {
    }

    public override Enum DefaultValue { get; set; } = EquipmentSuggestToolModeCategory.Crafting;
    public override string Key { get; set; } = "ToolModeCategory";
    public override string Name { get; set; } = LocalizationService.Ui("Category");
    public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("The category to use when in tool mode"));
    public override string Version { get; set; } = "12.0.10";

    public override Dictionary<Enum, string> Choices { get; } = new()
    {
        { EquipmentSuggestToolModeCategory.Crafting, LocalizationService.Ui("Crafting") },
        { EquipmentSuggestToolModeCategory.Gathering, LocalizationService.Ui("Gathering") },
        { EquipmentSuggestToolModeCategory.Combat, LocalizationService.Ui("Combat") },
        { EquipmentSuggestToolModeCategory.CombatTank, LocalizationService.Ui("Combat (Tank)") },
        { EquipmentSuggestToolModeCategory.CombatHealer, LocalizationService.Ui("Combat (Healer)") },
        { EquipmentSuggestToolModeCategory.CombatMelee, LocalizationService.Ui("Combat (Melee)") },
        { EquipmentSuggestToolModeCategory.CombatRanged, LocalizationService.Ui("Combat (Ranged)") },

    };

    public override bool Equal(Enum item1, Enum item2)
    {
        return item1.Equals(item2);
    }
}