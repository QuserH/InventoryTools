using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Localization;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public class CraftDefaultHQRequiredFilter : BooleanFilter
{
    public override bool? DefaultValue { get; set; } = false;
    public override string Key { get; set; } = "CraftDefaultHqRequired";
    public override string Name { get; set; } = "需要优质品";
    public override string HelpText { get; set; } = "列表中的每件物品是否都要求HQ形态（如适用）？";
    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Settings;
    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return null;
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        return null;
    }

    public override string[] GetChoices()
    {
        return ["Yes", "No"];
    }

    public override void UpdateFilterConfiguration(FilterConfiguration configuration, bool? newValue)
    {
        configuration.CraftList.HQRequired = newValue ?? false;
        configuration.NotifyConfigurationChange();
    }

    public override bool? CurrentValue(FilterConfiguration configuration)
    {
        return configuration.CraftList.HQRequired;
    }

    public CraftDefaultHQRequiredFilter(ILogger<CraftDefaultHQRequiredFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}