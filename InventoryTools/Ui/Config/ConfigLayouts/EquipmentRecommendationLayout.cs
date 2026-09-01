using InventoryTools.EquipmentSuggest;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class EquipmentRecommendationLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return Page("equipment-recommendation", LocalizationService.Ui("Equipment Recommendation"),
            Paragraph(LocalizationService.Ui("Compares what you are wearing against what you could be wearing and suggests upgrades. These are the defaults the recommendation screen opens with.")),
            Section("Defaults",
                Setting<EquipmentSuggestModeSetting>(LocalizationService.Ui("Recommend by")),
                Setting<EquipmentSuggestViewModeSetting>("Layout"))
        );
    }
}