using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftReportPlaySoundFilter : GenericBooleanFilter
{
    public CraftReportPlaySoundFilter(ILogger<CraftReportPlaySoundFilter> logger, ImGuiService imGuiService) : base(
        "CraftReportPlaySound", "物品完成时播放音效？",
        "列表中物品达到所需数量时播放音效。",
        FilterCategory.Notifications, null, null, logger, imGuiService)
    {
        DefaultValue = false;
        AvailableIn = FilterType.CraftFilter;
    }
}