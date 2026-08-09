using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public enum ImGuiTooltipMode
{
    Never,
    Icons,
    Everywhere
}

public class ImGuiTooltipModeSetting : GenericEnumChoiceSetting<ImGuiTooltipMode>
{
    public ImGuiTooltipModeSetting(ILogger<ImGuiTooltipModeSetting> logger,
        ImGuiService imGuiService) : base("ImGuiTooltipMode",
        LocalizationService.Ui("Item Tooltip Mode"),
        LocalizationService.Ui("Should a tooltip for items be shown, never, when hovering an item's icon or when hovering any row within an item table?"),
        ImGuiTooltipMode.Icons,
        new Dictionary<ImGuiTooltipMode, string>()
        {
            {ImGuiTooltipMode.Never, "Never"},
            {ImGuiTooltipMode.Icons, "Icons"},
            {ImGuiTooltipMode.Everywhere, "Everywhere"},
        },
        SettingCategory.General,
        SettingSubCategory.General,
        "1.11.0.8",
        logger,
        imGuiService)
    {
    }
}