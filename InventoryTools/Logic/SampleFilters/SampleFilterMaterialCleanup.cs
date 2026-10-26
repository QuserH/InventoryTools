using System;
using System.Collections.Generic;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Editors;
using InventoryTools.Logic.Filters;
using InventoryTools.Logic.ItemRenderers;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using InventoryTools.Services.Interfaces;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Features;

public class SampleFilterMaterialCleanup : BooleanSetting, ISampleFilter
{
    private readonly IListService _listService;
    private readonly Func<ItemInfoRenderCategory, GenericHasSourceCategoryFilter> _hasSourceCategoryFactory;
    private readonly FilterConfiguration.Factory _filterConfigFactory;
    private readonly SourceInventoriesFilter _sourceInventoriesFilter;
    private readonly DestinationInventoriesFilter _destinationInventoriesFilter;
    private readonly HighlightWhenFilter _highlightWhenFilter;

    public SampleFilterMaterialCleanup(ILogger<SampleFilterMaterialCleanup> logger, ImGuiService imGuiService,
        IListService listService, Func<ItemInfoRenderCategory, GenericHasSourceCategoryFilter> hasSourceCategoryFactory,
        FilterConfiguration.Factory filterConfigFactory, SourceInventoriesFilter sourceInventoriesFilter,
        DestinationInventoriesFilter destinationInventoriesFilter, HighlightWhenFilter highlightWhenFilter) : base(logger, imGuiService)
    {
        _listService = listService;
        _hasSourceCategoryFactory = hasSourceCategoryFactory;
        _filterConfigFactory = filterConfigFactory;
        _sourceInventoriesFilter = sourceInventoriesFilter;
        _destinationInventoriesFilter = destinationInventoriesFilter;
        _highlightWhenFilter = highlightWhenFilter;
    }
    private bool _shouldAdd;
    public override bool DefaultValue { get; set; }
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return _shouldAdd;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        _shouldAdd = newValue;
    }

    public override string Key { get; set; } = "sample3";
    public override string Name { get; set; } = "清理素材";
    public override string HelpText { get; set; } = "查找角色背包中所有可采集物品，并尝试显示应将它们放入雇员的哪些位置。";
    public string SampleDefaultName => "清理素材";
    public string SampleDescription =>
        "将添加一个用于快速收纳多余素材的列表。将自动添加全部素材分类。计算物品放置位置时，会优先考虑已有物品的堆叠。";
    public SampleFilterType SampleFilterType => SampleFilterType.Sample;
    public override SettingCategory SettingCategory { get; set; } = SettingCategory.None;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.None;
    public override string Version => "1.7.0.0";
    public bool ShouldAdd => _shouldAdd;
    public FilterConfiguration AddFilter()
    {
        var sampleFilter = _filterConfigFactory.Invoke();
        sampleFilter.Name = Name;
        sampleFilter.FilterType = FilterType.SortingFilter;
        sampleFilter.DisplayInTabs = true;
        _sourceInventoriesFilter.UpdateFilterConfiguration(sampleFilter, [
            new InventorySearchScope()
            {
                ActiveCharacter = true,
                Categories = [InventoryCategory.CharacterBags]
            }
        ]);
        _destinationInventoriesFilter.UpdateFilterConfiguration(sampleFilter, [
            new InventorySearchScope()
            {
                ActiveCharacter = true,
                Categories = [InventoryCategory.RetainerBags]
            }
        ]);
        _highlightWhenFilter.UpdateFilterConfiguration(sampleFilter, HighlightWhen.Always);

        sampleFilter.FilterItemsInRetainersEnum = FilterItemsRetainerEnum.Yes;
        var gatherFilter = _hasSourceCategoryFactory.Invoke(ItemInfoRenderCategory.Gathering);
        gatherFilter.UpdateFilterConfiguration(sampleFilter, true);
        _listService.AddDefaultColumns(sampleFilter);
        _listService.AddList(sampleFilter);
        return sampleFilter;
    }
}
