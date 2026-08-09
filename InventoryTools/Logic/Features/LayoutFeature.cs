using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Features;

public class LayoutFeature : Feature
{
    public LayoutFeature(IEnumerable<ISetting> settings) : base(new[]
        {
            typeof(CraftWindowLayoutSetting),
            typeof(FiltersWindowLayoutSetting),
        },
        settings)
    {
    }

    public override string Name { get; } = LocalizationService.Ui("Layout");
    public override string Description { get; } =
        LocalizationService.Ui("How should the main items window and craft windows be laid out? Should we display your lists as tabs or in a side bar?");
}