using System;
using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using Lumina.Excel.Sheets;
using InventoryTools.Localization;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemCustomDeliverySourceRenderer : ItemInfoRenderer<ItemCustomDeliverySource>
{
    public ItemCustomDeliverySourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.CustomDelivery;
    public override string SingularName => "老主顾";
    public override string PluralName => "老主顾";
    public override string HelpText => "该物品是否可用于老主顾交付任务？";
    public override bool ShouldGroup => false;

    public override Func<ItemSource, (Type, uint)>? RelatedType => source =>
    {
        var asSource = AsSource(source);
        return (typeof(SatisfactionNpc), asSource.SupplyRow.Npc!.Base.RowId);
    };

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        var eNpcResident = asSource.SupplyRow.Npc?.Base.Npc.Value;
        if (eNpcResident != null)
        {
            var collectabilityLow = asSource.SupplyRow.Base.CollectabilityLow;
            var collectabilityMid = asSource.SupplyRow.Base.CollectabilityMid;
            var collectabilityHigh = asSource.SupplyRow.Base.CollectabilityHigh;
            ImGui.Text("NPC: " + eNpcResident.Value.Singular.ExtractText());
            ImGui.Text("收藏价值（低）：" + collectabilityLow);
            ImGui.Text("收藏价值（中）：" + collectabilityMid);
            ImGui.Text("收藏价值（高）：" + collectabilityHigh);
        }
        else
        {
            ImGui.Text("未知 NPC");
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        var eNpcResident = asSource.SupplyRow.Npc?.Base.Npc.Value;
        if (eNpcResident != null)
        {
            return eNpcResident.Value.Singular.ExtractText();
        }

        return "";
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.CustomDeliveryIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        var eNpcResident = asSource.SupplyRow.Npc?.Base.Npc.Value;
        if (eNpcResident != null)
        {
            var collectabilityLow = asSource.SupplyRow.Base.CollectabilityLow;
            var collectabilityMid = asSource.SupplyRow.Base.CollectabilityMid;
            var collectabilityHigh = asSource.SupplyRow.Base.CollectabilityHigh;
            return $"{eNpcResident.Value.Singular.ExtractText()} ({collectabilityLow}, {collectabilityMid}, {collectabilityHigh})";
        }

        return "Unknown NPC";
    };
}