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
        "物品悬浮提示模式",
        "物品悬浮提示是否应显示？从不、悬停物品图标时、还是悬停物品表格中的任意行时？",
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