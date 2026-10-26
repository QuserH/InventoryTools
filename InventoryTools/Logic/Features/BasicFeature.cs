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
    
    public override string Name { get; } = "基础";
    public override string Description { get; } = "配置亚拉戈工具箱的基础设置";
}