using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class CompendiumWotsitSetting : GenericBooleanSetting
{
    public CompendiumWotsitSetting(ILogger<CompendiumWotsitSetting> logger, ImGuiService imGuiService) : base("CompendiumWotsitSetting", "启用 Wotsit 整合？", "图鉴窗口是否应列入 Wotsit 的搜索？", true, SettingCategory.Compendium, SettingSubCategory.General, "14.1.0", logger, imGuiService)
    {
    }
}