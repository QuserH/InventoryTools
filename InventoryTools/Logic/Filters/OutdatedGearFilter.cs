using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using InventoryTools.Localization;

using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public class OutdatedGearFilter : BooleanFilter
{
    private readonly IGameInteropService _gameInteropService;
    private Dictionary<uint, short>? _jobClassLevels;

    public OutdatedGearFilter(ILogger<OutdatedGearFilter> logger, ImGuiService imGuiService, IGameInteropService gameInteropService) : base(logger, imGuiService)
    {
        _gameInteropService = gameInteropService;
    }


    public override string Key { get; set; } = "OutdatedGearFilter";
    public override string Name { get; set; } = "过时装备？";
    public override string HelpText { get; set; } = "显示所有被认为过时的装备。将每个物品的品级与你各职业的等级进行比较，使用你拥有的武器相应职业中的最低等级来判断是否过时。未拥有的职业不计入考量。";
    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Basic;

    public override FilterType AvailableIn { get; set; } = FilterType.SearchFilter | FilterType.SortingFilter | FilterType.GameItemFilter | FilterType.HistoryFilter;

    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return FilterItem(configuration, item.Item);
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        var currentValue = this.CurrentValue(configuration);
        if (currentValue == null)
        {
            return null;
        }

        var isOutdated = false;
        var jobClassLevels = GetClassJobLevels();

        if (item.ClassJobCategory != null)
        {
            int? lowestJobLevel = null;

            foreach (var job in item.ClassJobCategory.ClassJobs)
            {
                if (jobClassLevels.TryGetValue(job.RowId, out var jobLevel))
                {
                    if (lowestJobLevel == null || lowestJobLevel > jobLevel)
                    {
                        lowestJobLevel = jobLevel;
                    }
                }
            }

            if (lowestJobLevel != null && lowestJobLevel > item.Base.LevelEquip)
            {
                isOutdated = true;
            }
        }
        else
        {
            return false;
        }

        switch (currentValue)
        {
            case true when isOutdated:
            case false when !isOutdated:
                return true;
            default:
                return false;
        }
    }


    private Dictionary<uint, short> GetClassJobLevels()
    {
        if (_jobClassLevels == null)
        {
            _jobClassLevels = _gameInteropService.GetClassJobLevels()?.ToDictionary(c => c.Key.RowId, c=> c.Value) ?? new();
        }

        return _jobClassLevels;
    }

    public override void InvalidateSearchCache()
    {
        _jobClassLevels = null;
    }
}