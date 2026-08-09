using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Features;

public class BasicFeature : Feature
{
    public BasicFeature(IEnumerable<ISetting> settings) : base(new[]
        {
            typeof(AutoSaveSetting),
            typeof(AllowCrossCharacterSetting),
            typeof(HistoryEnabledSetting),
            typeof(AddTitleMenuButtonSetting)
        },
        settings)
    {
    }
    
    public override string Name { get; } = LocalizationService.Ui("Basic");
    public override string Description { get; } = LocalizationService.Ui("Configure the basic settings of Allagan Tools");
}