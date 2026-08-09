using System.Collections.Generic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipAmountOwnedSortSetting : ChoiceSetting<TooltipAmountOwnedSort>
{
    public TooltipAmountOwnedSortSetting(ILogger<TooltipAmountOwnedSortSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override TooltipAmountOwnedSort DefaultValue { get; set; } = TooltipAmountOwnedSort.Alphabetically;
    public override TooltipAmountOwnedSort CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipAmountOwnedSort;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, TooltipAmountOwnedSort newValue)
    {
        configuration.TooltipAmountOwnedSort = newValue;
    }

    public override string Key { get; set; } = "TooltipAmountOwnedSort";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Add Item Locations (Order)"));

    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("How should the items displayed in the tooltip that you own be ordered? None is included if you find the sorting to be non-performant."));

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.AddItemLocations;
    public override string Version { get; } = "1.7.0.17";

    public override Dictionary<TooltipAmountOwnedSort, string> Choices { get; } =
        new Dictionary<TooltipAmountOwnedSort, string>()
        {
            { TooltipAmountOwnedSort.Alphabetically, LocalizationService.Ui("Alphabetical Order(Character/Retainer/etc)") },
            { TooltipAmountOwnedSort.Categorically, LocalizationService.Ui("Alphabetical Order(Category)") },
            { TooltipAmountOwnedSort.Quantity, LocalizationService.Ui("Item Quantity") },
            { TooltipAmountOwnedSort.None, LocalizationService.Ui("No Order") },
        };
}

public enum TooltipAmountOwnedSort
{
    Alphabetically,
    Categorically,
    Quantity,
    None
}