using System.Collections.Generic;
using InventoryTools.Logic.Settings;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Features;

public class TooltipsFeature : Feature
{
    public TooltipsFeature(IEnumerable<ISetting> settings) : base(new[]
        {
            typeof(TooltipDisplayAmountOwnedSetting),
            typeof(TooltipMinimumMarketPriceSetting),
            typeof(TooltipDisplayUnlockSetting),
            typeof(TooltipSourceInformationEnabledSetting),
            typeof(TooltipUseInformationEnabledSetting),
            typeof(TooltipDisplayIngredientPatchSetting),
            typeof(TooltipDisplayCofferLootSetting),
            typeof(TooltipDisplayGlamourReadySetSetting),
        },
        settings)
    {
    }

    public override string Name { get; } = "悬浮提示";
    public override string Description { get; } =
        "亚拉戈工具箱可为物品悬浮提示添加额外信息。选择要在悬浮提示中显示的内容。如需进一步配置（包括更改各悬浮提示的颜色和专属设置），请打开设置窗口。";
}