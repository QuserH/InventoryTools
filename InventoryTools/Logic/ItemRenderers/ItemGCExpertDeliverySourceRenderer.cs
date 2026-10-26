using System;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using InventoryTools.Localization;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemGcExpertDeliverySourceRenderer : ItemInfoRenderer<ItemGCExpertDeliverySource>
{
    public ItemGcExpertDeliverySourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.GCExpertDelivery;
    public override string SingularName => "大国防联军稀有品筹备";
    public override string HelpText => "该物品是否可在大国防联军处作为稀有品筹备交付？";
    public override bool ShouldGroup => false;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        var sealsRewarded = asSource.SealsRewarded;
        ImGui.Text("军票奖励：" + sealsRewarded);
    };

    public override Func<ItemSource, string> GetName => _ =>
    {
        return "";
    };
    public override Func<ItemSource, int> GetIcon => _ => Icons.FlameSealIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        var sealsRewarded = asSource.SealsRewarded;
        return asSource.SealsRewarded.ToString();
    };
}