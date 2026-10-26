using System;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Editors;
using InventoryTools.Logic.Filters;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using InventoryTools.Services.Interfaces;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Features;

public class SampleFilter100GillOrLess : BooleanSetting, ISampleFilter
{
    private readonly IListService _listService;
    private readonly BuyFromVendorPriceFilter _buyFromVendorPriceFilter;
    private readonly FilterConfiguration.Factory _filterConfigFactory;
    private readonly Lazy<SourceInventoriesFilter> _sourceInventoriesFilter;

    public SampleFilter100GillOrLess(ILogger<SampleFilter100GillOrLess> logger, ImGuiService imGuiService, IListService listService, BuyFromVendorPriceFilter buyFromVendorPriceFilter, FilterConfiguration.Factory filterConfigFactory, Lazy<SourceInventoriesFilter> sourceInventoriesFilter) : base(logger, imGuiService)
    {
        _listService = listService;
        _buyFromVendorPriceFilter = buyFromVendorPriceFilter;
        _filterConfigFactory = filterConfigFactory;
        _sourceInventoriesFilter = sourceInventoriesFilter;
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

    public override string Key { get; set; } = "sample1";
    public override string Name { get; set; } = "100 金币以内";
    public string SampleDefaultName => "100 金币以内";

    public string SampleDescription =>
        "将添加一个列表，显示可从金币商店以 100 金币以下购买的物品。将同时检查角色和雇员背包。";

    public SampleFilterType SampleFilterType => SampleFilterType.Sample;
    public override string HelpText { get; set; } = "显示商店出售价格低于 100 金币的物品。";
    public override SettingCategory SettingCategory { get; set; } = SettingCategory.None;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.None;
    public override string Version => "1.7.0.0";
    public bool ShouldAdd => _shouldAdd;
    public FilterConfiguration AddFilter()
    {
        var sampleFilter = _filterConfigFactory.Invoke();
        sampleFilter.Name = Name;
        sampleFilter.FilterType = FilterType.SearchFilter;
        sampleFilter.DisplayInTabs = true;
        _sourceInventoriesFilter.Value.UpdateFilterConfiguration(sampleFilter, [
            new InventorySearchScope()
            {
                ActiveCharacter = true,
                CharacterTypes = [CharacterType.Character, CharacterType.Retainer, CharacterType.FreeCompanyChest]
            }
        ]);
        _buyFromVendorPriceFilter.UpdateFilterConfiguration(sampleFilter, "<=100");
        _listService.AddDefaultColumns(sampleFilter);
        _listService.AddList(sampleFilter);
        return sampleFilter;
    }
}