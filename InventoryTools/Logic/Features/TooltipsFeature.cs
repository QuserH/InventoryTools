using System.Collections.Generic;
using InventoryTools.Localization;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class TooltipsFeature : Feature
{
    public TooltipsFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/tooltips", LocalizationService.Ui("Tooltips"),
            Paragraph(LocalizationService.Ui("Allagan Tools can add extra lines to the game's item tooltips.")),
            Setting<TooltipDisplayAmountOwnedSetting>(LocalizationService.Ui("Where you own the item")),
            Setting<TooltipDisplayRetrieveAmountSetting>(LocalizationService.Ui("How many the active craft list still needs")),
            Setting<TooltipMinimumMarketPriceSetting>(LocalizationService.Ui("The market price")),
            Setting<TooltipDisplayUnlockSetting>(LocalizationService.Ui("Whether you have learned/unlocked the item")),
            Setting<TooltipSourceInformationEnabledSetting>(LocalizationService.Ui("Where the item comes from")),
            Setting<TooltipUseInformationEnabledSetting>(LocalizationService.Ui("What the item is used for")),
            Setting<TooltipDisplayIngredientPatchSetting>(LocalizationService.Ui("Which patch an ingredient is from")),
            Setting<TooltipDisplayCofferLootSetting>(LocalizationService.Ui("What a coffer can contain")),
            Setting<TooltipDisplayCuratedListsSetting>(LocalizationService.Ui("Whether the item is on one of your curated lists")),
            Setting<TooltipDisplayGlamourReadySetSetting>(LocalizationService.Ui("Whether the item completes an outfit")),
            Paragraph(LocalizationService.Ui("Each line has more options in the settings window, under Tooltips. The options include colours, the locations to search, and the display mode."))
        );
    }
}
