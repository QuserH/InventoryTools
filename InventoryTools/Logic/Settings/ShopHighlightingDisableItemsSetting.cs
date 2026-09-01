using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class ShopHighlightingDisableItemsSetting : GenericBooleanSetting
{
    public ShopHighlightingDisableItemsSetting(ILogger<ShopHighlightingDisableItemsSetting> logger, ImGuiService imGuiService) : base("ShopHighlightingDisableItems", LocalizationService.Ui("Shop Highlighting - Disable Items"), LocalizationService.Ui("When highlighting items in a shop, should the not highlighted items be disabled?"), false, "15.0.4", logger, imGuiService)
    {
    }
}