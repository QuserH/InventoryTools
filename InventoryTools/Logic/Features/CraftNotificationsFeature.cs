using System.Collections.Generic;
using System.Linq;
using InventoryTools.Logic.Filters;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.WizardSettings;
using InventoryTools.Services.Interfaces;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Features;

public class CraftNotificationsFeature : Feature
{
    private readonly CraftReportCompletionOnlyFilter _completionOnlyFilter;
    private readonly CraftNotificationsCompletionOnlySetting _completionOnlySetting;
    private readonly InventoryToolsConfiguration _configuration;
    private readonly IListService _listService;
    private readonly CraftReportPlaySoundFilter _playSoundFilter;
    private readonly CraftNotificationsPlaySoundSetting _playSoundSetting;
    private readonly CraftReportProgressFilter _progressFilter;
    private readonly CraftNotificationsReportToChatSetting _reportToChatSetting;

    public CraftNotificationsFeature(IEnumerable<ISetting> settings,
        CraftNotificationsReportToChatSetting reportToChatSetting,
        CraftNotificationsPlaySoundSetting playSoundSetting,
        CraftNotificationsCompletionOnlySetting completionOnlySetting,
        CraftReportProgressFilter progressFilter,
        CraftReportPlaySoundFilter playSoundFilter,
        CraftReportCompletionOnlyFilter completionOnlyFilter,
        IListService listService,
        InventoryToolsConfiguration configuration) : base([
            typeof(CraftNotificationsReportToChatSetting),
            typeof(CraftNotificationsPlaySoundSetting),
            typeof(CraftNotificationsCompletionOnlySetting)
        ],
        settings)
    {
        _reportToChatSetting = reportToChatSetting;
        _playSoundSetting = playSoundSetting;
        _completionOnlySetting = completionOnlySetting;
        _progressFilter = progressFilter;
        _playSoundFilter = playSoundFilter;
        _completionOnlyFilter = completionOnlyFilter;
        _listService = listService;
        _configuration = configuration;
    }

    public override string Name => "制作通知";

    public override string Description =>
        "配置亚拉戈工具箱在获取制作列表物品时如何通知你。默认制作列表和现有制作列表将应用这些通知设置。通知仅在制作列表启用时发生。可通过编辑各列表或默认制作列表的设置进一步配置此功能。";

    public override void OnFinish()
    {
        var reportToChat = _reportToChatSetting.CurrentValue(_configuration);
        var playSound = _playSoundSetting.CurrentValue(_configuration);
        var completionOnly = _completionOnlySetting.CurrentValue(_configuration);

        var lists = _listService.Lists
            .Where(c => c.FilterType == FilterType.CraftFilter)
            .ToList();
        var defaultList = _listService.GetDefaultCraftList();
        if (!lists.Contains(defaultList)) lists.Add(defaultList);

        foreach (var list in lists)
        {
            _progressFilter.UpdateFilterConfiguration(list, reportToChat);
            _playSoundFilter.UpdateFilterConfiguration(list, playSound);
            _completionOnlyFilter.UpdateFilterConfiguration(list, completionOnly);
        }
    }
}