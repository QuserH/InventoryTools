using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Features;

public class ContextMenuFeature : Feature
{
    public ContextMenuFeature(IEnumerable<ISetting> settings) : base(new[]
        {
            typeof(ContextMenuMoreInformationSetting),
            typeof(ContextMenuMoreInformationNpcsSetting),
            typeof(ContextMenuMoreInformationMonstersSetting),
            typeof(ContextMenuAddToCraftListSetting),
            typeof(ContextMenuAddToActiveCraftListSetting),
            typeof(ContextMenuAddToCuratedListSetting),
            typeof(ContextMenuAddToFavouritesSetting),
            typeof(ContextMenuOpenCraftingLogSetting),
            typeof(ContextMenuOpenGatheringLogSetting),
            typeof(ContextMenuOpenFishingLogSetting),
            typeof(ContextMenuCopyNameSetting),
        },
        settings)
    {
    }

    public override string Name { get; } = "快捷菜单";

    public override string Description { get; } =
        "为游戏内物品的右键/快捷菜单添加新条目。";
}