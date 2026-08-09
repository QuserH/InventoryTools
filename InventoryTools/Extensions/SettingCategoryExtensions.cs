using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;

namespace InventoryTools.Extensions
{
    public static class SettingCategoryExtensions
    {
        public static string FormattedName(this SettingCategory settingCategory)
        {
            switch (settingCategory)
            {
                case SettingCategory.General:
                    return LocalizationService.Ui("General");
                case SettingCategory.Visuals:
                    return LocalizationService.Ui("Visuals");
                case SettingCategory.MarketBoard:
                    return LocalizationService.Ui("Marketboard");
                case SettingCategory.CraftOverlay:
                    return LocalizationService.Ui("Craft Overlay");
                case SettingCategory.CraftTracker:
                    return LocalizationService.Ui("Craft Tracker (Legacy)");
                case SettingCategory.ToolTips:
                    return LocalizationService.Ui("Tooltips");
                case SettingCategory.Hotkeys:
                    return LocalizationService.Ui("Hotkeys");
                case SettingCategory.History:
                    return LocalizationService.Ui("History");
                case SettingCategory.Windows:
                    return LocalizationService.Ui("Windows");
                case SettingCategory.Lists:
                    return LocalizationService.Ui("Lists");
                case SettingCategory.ContextMenu:
                    return LocalizationService.Ui("Context Menu");
                case SettingCategory.MobSpawnTracker:
                    return LocalizationService.Ui("Mob Spawn Tracker");
                case SettingCategory.TitleMenuButtons:
                    return LocalizationService.Ui("Title Menu Button");
                case SettingCategory.AutoSave:
                    return LocalizationService.Ui("Auto Save");
                case SettingCategory.Items:
                    return LocalizationService.Ui("Items");
                case SettingCategory.Highlighting:
                    return LocalizationService.Ui("Highlighting");
                case SettingCategory.EquipmentRecommendation:
                    return LocalizationService.Ui("Equipment Recommendations");
            }
            return settingCategory.ToString();
        }
    }
}