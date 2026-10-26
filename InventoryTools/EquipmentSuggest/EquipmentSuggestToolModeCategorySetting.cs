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
    public override string Name { get; set; } = "分类";
    public override string HelpText { get; set; } = "工具模式下使用的类别";
    public override string Version { get; set; } = "12.0.10";

    public override Dictionary<Enum, string> Choices { get; } = new()
    {
        { EquipmentSuggestToolModeCategory.Crafting, "制作" },
        { EquipmentSuggestToolModeCategory.Gathering, "采集" },
        { EquipmentSuggestToolModeCategory.Combat, "战斗" },
        { EquipmentSuggestToolModeCategory.CombatTank, "战斗（坦克）" },
        { EquipmentSuggestToolModeCategory.CombatHealer, "战斗（治疗）" },
        { EquipmentSuggestToolModeCategory.CombatMelee, "战斗（近战）" },
        { EquipmentSuggestToolModeCategory.CombatRanged, "战斗（远程）" },

    };

    public override bool Equal(Enum item1, Enum item2)
    {
        return item1.Equals(item2);
    }
}