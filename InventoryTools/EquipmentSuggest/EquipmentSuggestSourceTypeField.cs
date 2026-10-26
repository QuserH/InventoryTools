using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.Interface.FormFields;
using InventoryTools.Services;
using InventoryTools.Localization;

namespace InventoryTools.EquipmentSuggest;

public sealed class EquipmentSuggestSourceTypeField : MultipleChoiceFormField<ItemInfoType, EquipmentSuggestConfig>
{
    private readonly Dictionary<ItemInfoType, string> _itemTypes;

    public EquipmentSuggestSourceTypeField(ImGuiService imGuiService, ItemInfoRenderService renderService) : base(imGuiService)
    {
        ShowResults = false;
        _itemTypes = Enum.GetValues<ItemInfoType>().Where(renderService.HasSourceRenderer).ToDictionary(c => c, c => renderService.GetSourceTypeName(c).Singular);
    }

    public override List<ItemInfoType> DefaultValue { get; set; } = new()
    {
        ItemInfoType.GilShop,
        ItemInfoType.CraftRecipe,
    };
    public override string Key { get; set; } = "SourceType";
    public override string Name { get; set; } = "包含来源";
    public override string HelpText { get; set; } = "推荐物品时应使用哪些来源？";
    public override string Version { get; set; } = "1.12.0.10";
    public override Dictionary<ItemInfoType, string> GetChoices(EquipmentSuggestConfig configuration)
    {
        return _itemTypes;
    }

    public override bool HideAlreadyPicked { get; set; } = true;
}