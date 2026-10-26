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
                    return "常规";
                case SettingCategory.Visuals:
                    return "外观";
                case SettingCategory.MarketBoard:
                    return "市场布告板";
                case SettingCategory.CraftOverlay:
                    return "制作进度悬浮窗";
                case SettingCategory.CraftTracker:
                    return "制作追踪（旧版）";
                case SettingCategory.ToolTips:
                    return "悬浮提示";
                case SettingCategory.Hotkeys:
                    return "快捷键";
                case SettingCategory.History:
                    return "历史";
                case SettingCategory.Windows:
                    return "窗口";
                case SettingCategory.Lists:
                    return "列表";
                case SettingCategory.ContextMenu:
                    return "快捷菜单";
                case SettingCategory.MobSpawnTracker:
                    return "怪物刷新追踪器";
                case SettingCategory.TitleMenuButtons:
                    return "标题菜单按钮";
                case SettingCategory.AutoSave:
                    return "自动保存";
                case SettingCategory.Items:
                    return "物品";
                case SettingCategory.Highlighting:
                    return "高亮显示";
                case SettingCategory.EquipmentRecommendation:
                    return "装备推荐";
                case SettingCategory.Misc:
                    return "杂项";
                case SettingCategory.Troubleshooting:
                    return "疑难解答";
                case SettingCategory.Compendium:
                    return "图鉴";
            }
            return settingCategory.ToString();
        }
    }
}