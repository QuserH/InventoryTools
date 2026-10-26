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

    public override string Name { get; } = "布局";
    public override string Description { get; } =
        "主物品窗口和制作窗口应如何布局？以标签页还是侧边栏显示你的列表？";
}