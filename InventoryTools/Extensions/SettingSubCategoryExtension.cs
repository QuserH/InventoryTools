using System;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;

namespace InventoryTools.Extensions
{
    public static class SettingSubCategoryExtensions
    {
        public static string FormattedName(this SettingSubCategory settingSubCategory)
        {
            switch (settingSubCategory)
            {
                case SettingSubCategory.Experimental:
                    return LocalizationService.Ui("Experimental");
                case SettingSubCategory.Fun:
                    return LocalizationService.Ui("Fun");
                case SettingSubCategory.Highlighting:
                    return LocalizationService.Ui("Highlighting");
                case SettingSubCategory.DestinationHighlighting:
                    return LocalizationService.Ui("Destination Highlighting");
                case SettingSubCategory.RetainerHighlighting:
                    return LocalizationService.Ui("Retainer Highlighting");
                case SettingSubCategory.Market:
                    return LocalizationService.Ui("Market");
                case SettingSubCategory.General:
                    return LocalizationService.Ui("General");
                case SettingSubCategory.Subsetting:
                    return LocalizationService.Ui("Settings");
                case SettingSubCategory.Visuals:
                    return LocalizationService.Ui("Visuals");
                case SettingSubCategory.WindowLayout:
                    return LocalizationService.Ui("Window Layout");
                case SettingSubCategory.AutoSave:
                    return LocalizationService.Ui("Auto Save");
                case SettingSubCategory.FilterSettings:
                    return LocalizationService.Ui("List Settings");
                case SettingSubCategory.ActiveLists:
                    return LocalizationService.Ui("Active Lists");
                case SettingSubCategory.ContextMenus:
                    return LocalizationService.Ui("Context/Right Click Menu");
                case SettingSubCategory.Hotkeys:
                    return LocalizationService.Ui("Hotkeys");
                case SettingSubCategory.IgnoreEscape:
                    return LocalizationService.Ui("Ignore Escape Key");
                case SettingSubCategory.SourceGrouping:
                    return LocalizationService.Ui("Source Grouping");
                case SettingSubCategory.UseGrouping:
                    return LocalizationService.Ui("Use Grouping");
                case SettingSubCategory.Colours:
                    return LocalizationService.Ui("Colours");
                case SettingSubCategory.AddItemLocations:
                    return LocalizationService.Ui("Add Item Locations");
                case SettingSubCategory.MarketPricing:
                    return LocalizationService.Ui("Market Pricing");
                case SettingSubCategory.AmountToRetrieve:
                    return LocalizationService.Ui("Amount To Retrieve");
                case SettingSubCategory.ItemUnlockStatus:
                    return LocalizationService.Ui("Item Unlock Status");
                case SettingSubCategory.SourceInformation:
                    return LocalizationService.Ui("Source Information");
                case SettingSubCategory.UseInformation:
                    return LocalizationService.Ui("Use Information");
                case SettingSubCategory.AcquisitionTracker:
                    return LocalizationService.Ui("Acquisition Tracker");
                case SettingSubCategory.IngredientPatch:
                    return LocalizationService.Ui("Ingredient Patch");
                case SettingSubCategory.GlamourReadySet:
                    return LocalizationService.Ui("Outfit Glamours");
                case SettingSubCategory.CofferLoot:
                    return LocalizationService.Ui("Coffer Loot");
                case SettingSubCategory.ShopHighlighting:
                    return LocalizationService.Ui("Shop Highlighting");
            }
            return settingSubCategory.ToString();
        }
    }
}