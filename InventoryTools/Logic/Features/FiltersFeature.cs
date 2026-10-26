using System.Collections.Generic;
using System.Linq;
using Autofac;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Features;

public class FiltersFeature : Feature
{
    public FiltersFeature(IEnumerable<ISetting> settings) : base(new[]
        {
            typeof(SampleFilter100GillOrLess),
            typeof(SampleFilterDuplicateItems),
            typeof(SampleFilterMaterialCleanup),
        },
        settings)
    {
    }

    public override string Name { get; } = "示例物品列表";
    public override string Description { get; } = "选择默认安装的示例物品列表。这些是亚拉戈工具箱中可创建的列表类型的良好示例。";

    public override void OnFinish()
    {
        foreach (var setting in RelatedSettings.Select(c => c as ISampleFilter))
        {
            if (setting != null && setting.ShouldAdd)
            {
                setting.AddFilter();
            }
        }
    }

}

//Need to add in hide category or make category optional/null
//Need to add in put in armoire/glamour sample
//Maybe other samples?
