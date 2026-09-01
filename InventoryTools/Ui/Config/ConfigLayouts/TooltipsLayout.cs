using InventoryTools.Logic.Settings;
using InventoryTools.Ui.Config.Layouts;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Config.ConfigLayouts;

public class TooltipsLayout : ConfigLayout
{
    public override PageLayout Build()
    {
        return PageGroup("tooltips", "Tooltips",
            Page(LocalizationService.Ui("tooltips/general"), "General",
                Paragraph(LocalizationService.Ui("Allagan Tools can add extra lines to the game's item tooltips. Every feature in this section is inactive unless tooltip tweaks are enabled.")),
                Setting<ShowTooltipsSetting>(LocalizationService.Ui("Enable tooltip tweaks")),
                EnabledBy<ShowTooltipsSetting>(
                    Setting<ImGuiTooltipModeSetting>(LocalizationService.Ui("Show in Allagan Tools' own windows")),
                    Setting<TooltipDisplayHeaderSetting>(LocalizationService.Ui("Label added lines with the plugin name")),
                    Setting<TooltipCategoryWhitelistSetting>(LocalizationService.Ui("Limit to these item categories")),
                    Setting<TooltipCategoryBlacklistSetting>(LocalizationService.Ui("Treat the list above as exclusions")))),
            Page(LocalizationService.Ui("tooltips/appearance"), "Appearance",
                Paragraph(LocalizationService.Ui("Defaults for the lines Allagan Tools adds. Each feature can override the colour.")),
                Setting<TooltipColorSetting>(LocalizationService.Ui("Default text colour")),
                Setting<TooltipHeaderLinesSetting>(LocalizationService.Ui("Blank lines above")),
                Setting<TooltipFooterLinesSetting>(LocalizationService.Ui("Blank lines below"))),
            Page(LocalizationService.Ui("tooltips/locations"), LocalizationService.Ui("Item locations"),
                Paragraph(LocalizationService.Ui("Where you already own this item, and how many.")),
                Setting<TooltipDisplayAmountOwnedSetting>(LocalizationService.Ui("Show where I own this")),
                EnabledBy<TooltipDisplayAmountOwnedSetting>(
                    Setting<TooltipLocationScopeLimitSetting>(LocalizationService.Ui("Search these locations")),
                    Setting<TooltipLocationDisplayModeSetting>(LocalizationService.Ui("Display mode")),
                    Setting<TooltipAmountOwnedSortSetting>("Order"),
                    Setting<ToolTipLocationLimitSetting>(LocalizationService.Ui("Maximum results")),
                    Setting<TooltipCurrentCharacterSetting>(LocalizationService.Ui("Current character only")),
                    Setting<TooltipAddCharacterNameSetting>(LocalizationService.Ui("Affix the character name")),
                    Setting<TooltipAmountOwnedColorSetting>(LocalizationService.Ui("Text colour")))),
            Page(LocalizationService.Ui("tooltips/retrieve"), LocalizationService.Ui("Amount to retrieve"),
                Paragraph(LocalizationService.Ui("How many of this item your active list(sort list or craft list) still wants you to retrieve.")),
                Setting<TooltipDisplayRetrieveAmountSetting>(LocalizationService.Ui("Show amount to retrieve")),
                EnabledBy<TooltipDisplayRetrieveAmountSetting>(
                    Setting<TooltipAmountToRetrieveColorSetting>(LocalizationService.Ui("Text colour")))),
            Page(LocalizationService.Ui("tooltips/market"), LocalizationService.Ui("Market prices"),
                Paragraph(LocalizationService.Ui("Universalis pricing for the item. Either line can be shown on its own.")),
                Setting<TooltipAverageMarketPriceSetting>(LocalizationService.Ui("Show average NQ/HQ price")),
                Setting<TooltipMinimumMarketPriceSetting>(LocalizationService.Ui("Show minimum NQ/HQ price")),
                Setting<TooltipMarketPricingColorSetting>(LocalizationService.Ui("Text colour"))),
            Page(LocalizationService.Ui("tooltips/unlock"), LocalizationService.Ui("Item unlock status"),
                Paragraph(LocalizationService.Ui("Whether the item has been learned, and by whom.")),
                Setting<TooltipDisplayUnlockSetting>(LocalizationService.Ui("Show unlock status")),
                EnabledBy<TooltipDisplayUnlockSetting>(
                    Setting<TooltipDisplayUnlockCharacterSetting>(LocalizationService.Ui("Characters to check")),
                    Setting<TooltipDisplayUnlockDisplayModeSetting>(LocalizationService.Ui("Display mode")),
                    Setting<TooltipDisplayUnlockHideUnlockedSetting>(LocalizationService.Ui("Hide characters who have it")),
                    Setting<TooltipItemUnlockStatusColorSetting>(LocalizationService.Ui("Text colour")))),
            Page(LocalizationService.Ui("tooltips/coffer"), LocalizationService.Ui("Coffer loot"),
                Paragraph(LocalizationService.Ui("For coffers and containers, what they can contain.")),
                Setting<TooltipDisplayCofferLootSetting>(LocalizationService.Ui("Show coffer contents")),
                EnabledBy<TooltipDisplayCofferLootSetting>(
                    Setting<TooltipCofferLootScopeSetting>(LocalizationService.Ui("Search these locations")),
                    Setting<TooltipCofferLootColorSetting>(LocalizationService.Ui("Text colour")))),
            Page(LocalizationService.Ui("tooltips/curated"), LocalizationService.Ui("Curated lists"),
                Paragraph(LocalizationService.Ui("Which of your curated lists already contain this item.")),
                Setting<TooltipDisplayCuratedListsSetting>(LocalizationService.Ui("Show curated lists")),
                EnabledBy<TooltipDisplayCuratedListsSetting>(
                    Setting<TooltipCuratedListsSetting>(LocalizationService.Ui("Limit to these lists")),
                    Setting<TooltipCuratedListsMatchQualitySetting>(LocalizationService.Ui("Match item quality")),
                    Setting<TooltipCuratedListsColorSetting>(LocalizationService.Ui("Text colour")))),
            Page(LocalizationService.Ui("tooltips/glamour"), LocalizationService.Ui("Outfit glamour"),
                Paragraph(LocalizationService.Ui("Whether this item completes an outfit you are collecting.")),
                Setting<TooltipDisplayGlamourReadySetSetting>(LocalizationService.Ui("Show outfit glamour info")),
                EnabledBy<TooltipDisplayGlamourReadySetSetting>(
                    Setting<TooltipGlamourReadySetScopeSetting>(LocalizationService.Ui("Search these locations")),
                    Setting<TooltipGlamourReadySetDisplayModeSetting>(LocalizationService.Ui("Display mode")),
                    Setting<TooltipGlamourReadySetColorSetting>(LocalizationService.Ui("Text colour")),
                    Setting<TooltipGlamourReadySetAcquiredColorSetting>(LocalizationService.Ui("Acquired item colour")),
                    Setting<TooltipGlamourReadySetNotAcquiredColorSetting>(LocalizationService.Ui("Not acquired item colour")))),
            Page(LocalizationService.Ui("tooltips/patch"), LocalizationService.Ui("Ingredient patch"),
                Paragraph(LocalizationService.Ui("Which patch a crafting ingredient was introduced in.")),
                Setting<TooltipDisplayIngredientPatchSetting>(LocalizationService.Ui("Show ingredient patch")),
                EnabledBy<TooltipDisplayIngredientPatchSetting>(
                    Setting<TooltipIngredientPatchTooltipColorSetting>(LocalizationService.Ui("Text colour")))),
            Page(LocalizationService.Ui("tooltips/sources"), LocalizationService.Ui("Source information"),
                Paragraph(LocalizationService.Ui("How the item can be acquired.")),
                Setting<TooltipSourceInformationEnabledSetting>(LocalizationService.Ui("Show source information")),
                EnabledBy<TooltipSourceInformationEnabledSetting>(
                    Setting<TooltipSourceInformationModifierSetting>(LocalizationService.Ui("Hold this key to show")),
                    Setting<TooltipSourceInformationColorSetting>(LocalizationService.Ui("Text colour")),
                    Scrollable("sourceInformation", 260,
                        Setting<TooltipSourceInformationSetting>()))),
            Page(LocalizationService.Ui("tooltips/uses"), LocalizationService.Ui("Use information"),
                Paragraph(LocalizationService.Ui("What the item can be turned into or spent on.")),
                Setting<TooltipUseInformationEnabledSetting>(LocalizationService.Ui("Show use information")),
                EnabledBy<TooltipUseInformationEnabledSetting>(
                    Setting<TooltipUseInformationModifierSetting>(LocalizationService.Ui("Hold this key to show")),
                    Setting<TooltipUseInformationColorSetting>(LocalizationService.Ui("Text colour")),
                    Scrollable("useInformation", 260,
                        Setting<TooltipUseInformationSetting>())))
        );
    }
}