using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipLocationDisplayModeSetting : ChoiceSetting<TooltipLocationDisplayMode>
{
    public override TooltipLocationDisplayMode DefaultValue { get; set; } =
        TooltipLocationDisplayMode.CharacterCategoryQuantityQuality;
    public override TooltipLocationDisplayMode CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipLocationDisplayMode;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, TooltipLocationDisplayMode newValue)
    {
        configuration.TooltipLocationDisplayMode = newValue;
    }

    public override string Key { get; set; } = "TooltipLocationDisplayMode";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Add Item Locations (Display Mode)"));

    public override string WizardName { get; } = LocalizationService.Ui("Display Mode");

    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("How the locations of items should be presented in the tooltip. This requires 'Add Item Locations?' to be on."));

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.AddItemLocations;

    public override Dictionary<TooltipLocationDisplayMode, string> Choices
    {
        get
        {
            return new Dictionary<TooltipLocationDisplayMode, string>()
            {
                { TooltipLocationDisplayMode.CharacterQuantityQuality, LocalizationService.Ui("Character/Retainer - Quantity - Quality") },
                { TooltipLocationDisplayMode.CharacterBagSlotQuality, LocalizationService.Ui("Character/Retainer - Bag - Slot - Quality") },
                { TooltipLocationDisplayMode.CharacterBagSlotQuantity, LocalizationService.Ui("Character/Retainer - Bag - Slot - Quantity") },
                {
                    TooltipLocationDisplayMode.CharacterCategoryQuantityQuality,
                    LocalizationService.Ui("Character/Retainer - Category - Quantity - Quality")
                },
                {
                    TooltipLocationDisplayMode.CharacterWorldCategoryQuantityQuality,
                    LocalizationService.Ui("Character/Retainer - World - Category - Quantity - Quality")
                },
            };
        }
    }
    public override string Version => "1.7.0.0";

    public TooltipLocationDisplayModeSetting(ILogger<TooltipLocationDisplayModeSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}