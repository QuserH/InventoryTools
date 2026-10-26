using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.Shared.Time;
using CriticalCommonLib.Models;
using Dalamud.Interface.Colors;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using OtterGui.Raii;
using InventoryTools.Localization;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemMiningSourceRenderer : ItemGatheringSourceRenderer<ItemMiningSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Mining];
    public override string HelpText => "该物品是否可从普通采矿点采集？";
    public ItemMiningSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.Mining, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采矿";
}

public class ItemQuarryingSourceRenderer : ItemGatheringSourceRenderer<ItemQuarryingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Mining];
    public override string HelpText => "该物品是否可从普通采石点采集？";
    public ItemQuarryingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.Quarrying, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采石";
}

public class ItemLoggingSourceRenderer : ItemGatheringSourceRenderer<ItemLoggingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Botany];
    public override string HelpText => "该物品是否可从普通伐木点采集？";
    public ItemLoggingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet,mapSheet, seTime, ItemInfoType.Logging, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "伐木";
}

public class ItemHarvestingSourceRenderer : ItemGatheringSourceRenderer<ItemHarvestingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Botany];
    public override string HelpText => "该物品是否可从普通除草点采集？";

    public ItemHarvestingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.Harvesting, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采收";
}

public class ItemHiddenMiningSourceRenderer : ItemGatheringSourceRenderer<ItemHiddenMiningSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Mining, ItemInfoRenderCategory.HiddenGathering];
    public override string HelpText => "该物品是否可从隐藏采矿点采集？";

    public ItemHiddenMiningSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.HiddenMining, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采矿（隐藏）";
}

public class ItemHiddenQuarryingSourceRenderer : ItemGatheringSourceRenderer<ItemHiddenQuarryingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Mining, ItemInfoRenderCategory.HiddenGathering];
    public override string HelpText => "该物品是否可从隐藏采石点采集？";

    public ItemHiddenQuarryingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.HiddenQuarrying, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采掘（隐藏）";
}

public class ItemHiddenLoggingSourceRenderer : ItemGatheringSourceRenderer<ItemHiddenLoggingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Botany, ItemInfoRenderCategory.HiddenGathering];
    public override string HelpText => "该物品是否可从隐藏伐木点采集？";

    public ItemHiddenLoggingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.HiddenLogging, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "伐木（隐藏）";
}

public class ItemHiddenHarvestingSourceRenderer : ItemGatheringSourceRenderer<ItemHiddenHarvestingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Botany, ItemInfoRenderCategory.HiddenGathering];
    public override string HelpText => "该物品是否可从隐藏除草点采集？";

    public ItemHiddenHarvestingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.HiddenHarvesting, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采收（隐藏）";
}

public class ItemTimedMiningSourceRenderer : ItemGatheringSourceRenderer<ItemTimedMiningSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Mining, ItemInfoRenderCategory.TimedGathering];
    public override string HelpText => "该物品是否可从限时采矿点采集？";

    public ItemTimedMiningSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.TimedMining, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采矿（定时）";
}

public class ItemTimedQuarryingSourceRenderer : ItemGatheringSourceRenderer<ItemTimedQuarryingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Mining, ItemInfoRenderCategory.TimedGathering];
    public override string HelpText => "该物品是否可从限时采石点采集？";

    public ItemTimedQuarryingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.TimedQuarrying, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采掘（定时）";
}

public class ItemTimedLoggingSourceRenderer : ItemGatheringSourceRenderer<ItemTimedLoggingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Botany, ItemInfoRenderCategory.TimedGathering];
    public override string HelpText => "该物品是否可从限时伐木点采集？";

    public ItemTimedLoggingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.TimedLogging, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "伐木（定时）";
}

public class ItemTimedHarvestingSourceRenderer : ItemGatheringSourceRenderer<ItemTimedHarvestingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Botany, ItemInfoRenderCategory.TimedGathering];
    public override string HelpText => "该物品是否可从限时除草点采集？";

    public ItemTimedHarvestingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.TimedHarvesting, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采收（定时）";
}

public class ItemEphemeralMiningSourceRenderer : ItemGatheringSourceRenderer<ItemEphemeralMiningSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Mining, ItemInfoRenderCategory.EphemeralGathering];
    public override string HelpText => "该物品是否可从临时采矿点采集？";

    public ItemEphemeralMiningSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.EphemeralMining, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采矿（临时）";
}

public class ItemEphemeralQuarryingSourceRenderer : ItemGatheringSourceRenderer<ItemEphemeralQuarryingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Mining, ItemInfoRenderCategory.EphemeralGathering];
    public override string HelpText => "该物品是否可从临时采石点采集？";
    public ItemEphemeralQuarryingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.EphemeralQuarrying, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采掘（临时）";
}

public class ItemEphemeralLoggingSourceRenderer : ItemGatheringSourceRenderer<ItemEphemeralLoggingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.Botany, ItemInfoRenderCategory.EphemeralGathering];
    public override string HelpText => "该物品是否可从临时伐木点采集？";
    public ItemEphemeralLoggingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.EphemeralLogging, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "伐木（临时）";
}

public class ItemEphemeralHarvestingSourceRenderer : ItemGatheringSourceRenderer<ItemEphemeralHarvestingSource>
{
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Gathering, ItemInfoRenderCategory.EphemeralGathering];
    public override string HelpText => "该物品是否可从临时除草点采集？";

    public ItemEphemeralHarvestingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, seTime, ItemInfoType.EphemeralHarvesting, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "采收（临时）";
}

public abstract class ItemGatheringSourceRenderer<T> : ItemInfoRenderer<T> where T : ItemGatheringSource
{
    private readonly MapSheet _mapSheet;
    private readonly ISeTime _seTime;
    private readonly ItemInfoType _type;

    public ItemGatheringSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ISeTime seTime, ItemInfoType type,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
        _mapSheet = mapSheet;
        _seTime = seTime;
        _type = type;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => _type;
    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = (ItemGatheringSource)source;

         var level = asSource.GatheringItem.Base.GatheringItemLevel.Value.GatheringItemLevel;
         ImGui.Text("等级：" + (level == 0 ? "不适用" : level));
         var stars = asSource.GatheringItem.Base.GatheringItemLevel.Value.Stars;
         ImGui.Text("星级：" + (stars == 0 ? "不适用" : stars));
         var perceptionRequired = asSource.GatheringItem.Base.PerceptionReq;
         ImGui.Text("需要鉴别力：" +
                    (perceptionRequired == 0 ? "不适用" : perceptionRequired));

         if (asSource.GatheringItem.AvailableAtTimedNode)
         {
             ImGui.Text("地图：");
             using (ImRaii.PushIndent())
             {
                 foreach (var gatheringPoint in asSource.GatheringItem.GatheringPoints)
                 {
                     var mapName = _mapSheet.GetRow(gatheringPoint.Base.TerritoryType.Value.Map.RowId).FormattedName;
                     var nextUptime = gatheringPoint.GatheringPointTransient.GetGatheringUptime()
                         ?.NextUptime(_seTime.ServerTime) ?? null;
                     if (nextUptime == null
                         || nextUptime.Equals(TimeInterval.Always)
                         || nextUptime.Equals(TimeInterval.Invalid)
                         || nextUptime.Equals(TimeInterval.Never))
                     {
                         continue;
                     }
                     if (nextUptime.Value.Start > TimeStamp.UtcNow)
                     {
                         using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudRed))
                         {
                             ImGui.Text(mapName + "：将于 " +
                                               TimeInterval.DurationString(nextUptime.Value.Start, TimeStamp.UtcNow,
                                                   true));                         }
                     }
                     else
                     {
                         using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.HealerGreen))
                         {
                             ImGui.Text(mapName + " 已持续 " +
                                               TimeInterval.DurationString(nextUptime.Value.End, TimeStamp.UtcNow,
                                                   true));
                         }
                     }
                 }
             }
         }
         else
         {
             DrawMaps(source);
         }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = (ItemGatheringSource)source;
        return asSource.Item.NameString;
    };
    public override Func<ItemSource, int> GetIcon => _ =>
    {
        switch (_type)
        {
            case ItemInfoType.Mining:
                return Icons.MiningIcon;
            case ItemInfoType.Quarrying:
                return Icons.QuarryingIcon;
            case ItemInfoType.Harvesting:
                return Icons.HarvestingIcon;
            case ItemInfoType.Logging:
                return Icons.LoggingIcon;
            case ItemInfoType.TimedMining or ItemInfoType.HiddenMining
                or ItemInfoType.EphemeralMining:
                return Icons.TimedMiningIcon;
            case ItemInfoType.TimedQuarrying or ItemInfoType.HiddenQuarrying
                or ItemInfoType.EphemeralQuarrying:
                return Icons.TimedQuarryingIcon;
            case ItemInfoType.TimedHarvesting or ItemInfoType.HiddenHarvesting
                or ItemInfoType.EphemeralHarvesting:
                return Icons.TimedHarvestingIcon;
            case ItemInfoType.TimedLogging or ItemInfoType.HiddenLogging
                or ItemInfoType.EphemeralLogging:
                return Icons.TimedLoggingIcon;
        }

        return Icons.RedXIcon;
    };

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        var level = asSource.GatheringItem.Base.GatheringItemLevel.Value.GatheringItemLevel;
        var perceptionRequired = asSource.GatheringItem.Base.PerceptionReq;
        var stars = asSource.GatheringItem.Base.GatheringItemLevel.Value.Stars;
        var starsString = "";
        for (int i = 0; i < stars; i++)
        {
            starsString += "*";
        }

        return string.Format("等级 {0}（{1}）（需要感知力 {2}）",
            level == 0 ? LocalizationService.Ui("N/A") : level, starsString, perceptionRequired);
    };
}
