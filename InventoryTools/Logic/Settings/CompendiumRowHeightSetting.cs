using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class CompendiumRowHeightSetting : GenericIntegerSetting
{
    public CompendiumRowHeightSetting(ILogger<CompendiumRowHeightSetting> logger, ImGuiService imGuiService) : base("CompendiumRowHeight", "行高", "图鉴列表中行显示的最小高度应为何？", 32, SettingCategory.ContextMenu, SettingSubCategory.General, "14.0.3", logger, imGuiService)
    {
    }
}