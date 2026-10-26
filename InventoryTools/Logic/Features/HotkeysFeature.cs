using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Features;

public class HotkeysFeature : Feature
{
    public HotkeysFeature(IEnumerable<ISetting> settings) : base(new[]
        {
            typeof(HotkeyMoreInfoSetting),
            typeof(HotkeyConfigWindowSetting),
            typeof(HotkeyAirshipWindowSetting),
            typeof(HotkeyCraftWindowSetting),
            typeof(HotkeyDutiesWindowSetting),
            typeof(HotkeyMobWindowSetting),
            typeof(HotkeySubmarinesWindowSetting),
            typeof(HotkeyRetainerTasksWindowSetting),
            typeof(HotKeyListsWindowSetting),
            typeof(HotkeyOpenGatheringLogSetting),
            typeof(HotkeyOpenCraftingLogSetting),
            typeof(HotkeyOpenFishingLogSetting),
            typeof(HotkeyOpenItemLogSetting),
        },
        settings)
    {
    }

    public string Version { get; } = "1.0.0.0";
    public override string Name { get; } = "快捷键";

    public override string Description { get; } =
        "设置打开亚拉戈工具箱各窗口的快捷键。也可设置打开物品更多信息窗口的快捷键。";
}