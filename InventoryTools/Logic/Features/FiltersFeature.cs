using System.Collections.Generic;
using InventoryTools.Localization;
using System.Linq;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.Config;
using InventoryTools.Ui.Config.Layouts;

namespace InventoryTools.Logic.Features;

public class FiltersFeature : Feature
{
    public FiltersFeature(IEnumerable<ISetting> settings) : base(settings)
    {
    }

    public override PageLayout Build()
    {
        return Page("feature/sample-lists", LocalizationService.Ui("Sample Lists"),
            Paragraph(LocalizationService.Ui("These lists show you what the plugin can do. If you select them, they'll be installed once you complete the wizard.")),
            Setting<SampleFilter100GillOrLess>(LocalizationService.Ui("Items worth 100 gil or less")),
            Setting<SampleFilterDuplicateItems>(LocalizationService.Ui("Duplicate items in your inventory")),
            Setting<SampleFilterMaterialCleanup>(LocalizationService.Ui("Crafting materials to move to storage"))
        );
    }

    public override void OnFinish()
    {
        foreach (var setting in RelatedSettings.Select(c => c as ISampleFilter))
        {
            if (setting is { ShouldAdd: true })
            {
                setting.AddFilter();
            }
        }
    }
}
