using System.Collections.Generic;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Models;
using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters;

public class CraftReportSoundFilter : ChoiceFilter<uint>
{
    public override uint CurrentValue(FilterConfiguration configuration)
    {
        return configuration.GetUintFilter(Key) ?? DefaultValue;
    }
    
    public override void ResetFilter(FilterConfiguration configuration)
    {
        configuration.UpdateUintFilter(Key, DefaultValue);
    }
    
    public override void UpdateFilterConfiguration(FilterConfiguration configuration, uint newValue)
    {
        configuration.UpdateUintFilter(Key, newValue);
    }
    
    public override string Key { get; set; } = "CraftReportSound";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Completion Sound"));
    
    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("Which sound effect to play when an item in this list reaches its required quantity (when 'Play a sound when an item is complete?' is enabled)."));
    
    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Notifications;
    public override uint DefaultValue { get; set; } = 5;
    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    
    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return null;
    }
    
    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        return null;
    }
    
    public override List<uint> GetChoices(FilterConfiguration configuration)
    {
        return
        [
            1,
            2,
            3,
            4,
            5,
            6,
            7,
            8,
            9,
            10,
            11,
            12,
            13,
            14,
            15,
            16
        ];
    }
    
    public override string GetFormattedChoice(FilterConfiguration filterConfiguration, uint choice)
    {
        return LocalizationService.Ui("se.") + choice;
    }
    
    public CraftReportSoundFilter(ILogger<CraftReportSoundFilter> logger, ImGuiService imGuiService) : base(logger,
        imGuiService)
    {
    }
}