using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayIngredientPatchSetting : GenericBooleanSetting
{
    public TooltipDisplayIngredientPatchSetting(ILogger<TooltipDisplayIngredientPatchSetting> logger, ImGuiService imGuiService) : base("TooltipDisplayIngredientPatch", LocalizationService.Ui("Add Ingredient Patch"), LocalizationService.Ui("Displays the last patch an ingredient was used in. This is calculated by going each recipe an item is part of, then through it's recipes and so on until the highest patch is determined."), false, SettingCategory.ToolTips, SettingSubCategory.IngredientPatch, "1.12.0.11", logger, imGuiService)
    {
    }
}