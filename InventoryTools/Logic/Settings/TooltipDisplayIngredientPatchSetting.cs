using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayIngredientPatchSetting : GenericBooleanSetting
{
    public TooltipDisplayIngredientPatchSetting(ILogger<TooltipDisplayIngredientPatchSetting> logger, ImGuiService imGuiService) : base("TooltipDisplayIngredientPatch", "添加素材版本", "显示素材最后一次被使用的版本。计算方式为遍历物品所属的每个配方，再遍历其配方，直至确定最高版本。", false, SettingCategory.ToolTips, SettingSubCategory.IngredientPatch, "1.12.0.11", logger, imGuiService)
    {
    }
}