using System.Collections.Generic;
using AllaganLib.GameSheets.Sheets.Rows;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Models;
using InventoryTools.Localization;

using InventoryTools.Logic.Filters.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Filters;

public class RetainerRetrieveOrderFilter : ChoiceFilter<RetainerRetrieveOrder>
{
    public override RetainerRetrieveOrder CurrentValue(FilterConfiguration configuration)
    {
        if (configuration.FilterType == FilterType.CraftFilter)
        {
            return configuration.CraftList.RetainerRetrieveOrder;
        }

        return RetainerRetrieveOrder.RetrieveFirst;
    }

    public override FilterType AvailableIn { get; set; } = FilterType.CraftFilter;
    public override bool? FilterItem(FilterConfiguration configuration, InventoryItem item)
    {
        return null;
    }

    public override bool? FilterItem(FilterConfiguration configuration, ItemRow item)
    {
        return null;
    }

    public override void ResetFilter(FilterConfiguration configuration)
    {
        UpdateFilterConfiguration(configuration, RetainerRetrieveOrder.RetrieveFirst);
    }

    public override void UpdateFilterConfiguration(FilterConfiguration configuration, RetainerRetrieveOrder newValue)
    {
        if (configuration.FilterType == FilterType.CraftFilter)
        {
            configuration.CraftList.RetainerRetrieveOrder = newValue;
            configuration.NotifyConfigurationChange();
        }
    }

    public override string Key { get; set; } = "RetainerRetrieveOrder";
    public override string Name { get; set; } = "雇员取回顺序";
    public override string HelpText { get; set; } = "显示制作物品时，若有待取回物品，应在补足缺口前还是后显示。选择优先将先取回物品，选择靠后则需先收集/购买缺少的物品，之后再显示待取回的其余部分。";
    public override FilterCategory FilterCategory { get; set; } = FilterCategory.Settings;
    public override RetainerRetrieveOrder DefaultValue { get; set; } = RetainerRetrieveOrder.RetrieveFirst;
    public override List<RetainerRetrieveOrder> GetChoices(FilterConfiguration configuration)
    {
        return new List<RetainerRetrieveOrder>()
        {
            RetainerRetrieveOrder.RetrieveFirst,
            RetainerRetrieveOrder.RetrieveLast
        };
    }

    public override string GetFormattedChoice(FilterConfiguration filterConfiguration, RetainerRetrieveOrder choice)
    {
        switch (choice)
        {
            case RetainerRetrieveOrder.RetrieveFirst:
                return "优先取回";
            case RetainerRetrieveOrder.RetrieveLast:
                return "最后取回";
        }
        return "Unknown";
    }

    public RetainerRetrieveOrderFilter(ILogger<RetainerRetrieveOrderFilter> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}