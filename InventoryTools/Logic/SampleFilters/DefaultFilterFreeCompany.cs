using System.Collections.Generic;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Editors;
using InventoryTools.Logic.Filters;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services.Interfaces;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Features;

public class DefaultFilterFreeCompany : ISampleFilter
{
    private readonly FilterConfiguration.Factory _filterConfigFactory;
    private readonly SourceInventoriesFilter _sourceInventoriesFilter;
    private readonly IListService _listService;

    public DefaultFilterFreeCompany(FilterConfiguration.Factory filterConfigFactory, SourceInventoriesFilter sourceInventoriesFilter, IListService listService)
    {
        _filterConfigFactory = filterConfigFactory;
        _sourceInventoriesFilter = sourceInventoriesFilter;
        _listService = listService;
    }
    public bool ShouldAdd { get; set; }
    public FilterConfiguration AddFilter()
    {
        var allItemsFilter = _filterConfigFactory.Invoke();
        allItemsFilter.Name = SampleDefaultName;
        allItemsFilter.FilterType = FilterType.SearchFilter;
        allItemsFilter.DisplayInTabs = true;
        _sourceInventoriesFilter.UpdateFilterConfiguration(allItemsFilter, new List<InventorySearchScope>()
        {
            new()
            {
                ActiveCharacter = true,
                CharacterTypes = [CharacterType.FreeCompanyChest]
            }
        });
        _listService.AddDefaultColumns(allItemsFilter);
        _listService.AddList(allItemsFilter);
        return allItemsFilter;
    }

    public string Name => "部队";
    public string SampleDefaultName => "部队";

    public string SampleDescription =>
        "将添加一个预配置的列表，显示当前角色部队储物柜中的全部物品。";

    public SampleFilterType SampleFilterType => SampleFilterType.Default;
}