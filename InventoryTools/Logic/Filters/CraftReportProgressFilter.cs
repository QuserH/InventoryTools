using InventoryTools.Logic.GenericFilters;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftReportProgressFilter : GenericBooleanFilter
{
    public CraftReportProgressFilter(ILogger<CraftReportProgressFilter> logger, ImGuiService imGuiService) : base(
        "CraftReportProgress", "将获取进度报告到聊天频道？",
        "获取制作列表中的物品时，在聊天中输出进度（例如“铁木原木 剩余 30”）。制作列表必须处于启用状态。",
        FilterCategory.Notifications, null, null, logger, imGuiService)
    {
        DefaultValue = true;
        AvailableIn = FilterType.CraftFilter;
    }
}