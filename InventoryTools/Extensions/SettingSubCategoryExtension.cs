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
                    return "实验性";
                case SettingSubCategory.Fun:
                    return "娱乐";
                case SettingSubCategory.Highlighting:
                    return "高亮显示";
                case SettingSubCategory.DestinationHighlighting:
                    return "目标高亮";
                case SettingSubCategory.RetainerHighlighting:
                    return "雇员高亮";
                case SettingSubCategory.Market:
                    return "市场";
                case SettingSubCategory.General:
                    return "常规";
                case SettingSubCategory.Subsetting:
                    return "设置";
                case SettingSubCategory.Visuals:
                    return "外观";
                case SettingSubCategory.WindowLayout:
                    return "窗口布局";
                case SettingSubCategory.AutoSave:
                    return "自动保存";
                case SettingSubCategory.FilterSettings:
                    return "列表设置";
                case SettingSubCategory.ActiveLists:
                    return "启用的列表";
                case SettingSubCategory.ContextMenus:
                    return "右键菜单";
                case SettingSubCategory.Hotkeys:
                    return "快捷键";
                case SettingSubCategory.IgnoreEscape:
                    return "忽略 Esc 键";
                case SettingSubCategory.SourceGrouping:
                    return "来源分组";
                case SettingSubCategory.UseGrouping:
                    return "用途分组";
                case SettingSubCategory.Colours:
                    return "颜色";
                case SettingSubCategory.AddItemLocations:
                    return "添加物品位置";
                case SettingSubCategory.MarketPricing:
                    return "市场价格";
                case SettingSubCategory.AmountToRetrieve:
                    return "取回数量";
                case SettingSubCategory.ItemUnlockStatus:
                    return "物品解锁状态";
                case SettingSubCategory.SourceInformation:
                    return "来源信息";
                case SettingSubCategory.UseInformation:
                    return "用途信息";
                case SettingSubCategory.AcquisitionTracker:
                    return "获取追踪器";
                case SettingSubCategory.IngredientPatch:
                    return "素材版本";
                case SettingSubCategory.GlamourReadySet:
                    return "套装幻化";
                case SettingSubCategory.CofferLoot:
                    return "宝箱战利品";
                case SettingSubCategory.ShopHighlighting:
                    return "商店高亮";
            }
            return settingSubCategory.ToString();
        }
    }
}