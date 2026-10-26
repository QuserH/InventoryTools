using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public enum CraftWindowView
{
    Crafts,
    Tree,
    Configuration
}

public class CraftWindowViewSetting : GenericEnumChoiceSetting<CraftWindowView>
{
    public CraftWindowViewSetting(ILogger<CraftWindowViewSetting> logger, ImGuiService imGuiService) : base("CraftWindowView", "制作窗口视图", "制作列表当前处于哪种视图？",CraftWindowView.Crafts, new Dictionary<CraftWindowView, string>(){
        { CraftWindowView.Crafts , "Crafts"}, { CraftWindowView.Configuration, "Configuration"},
        { CraftWindowView.Tree, "Treeview"}}, SettingCategory.None, SettingSubCategory.None, "12.0.20", logger, imGuiService)
    {
    }
}