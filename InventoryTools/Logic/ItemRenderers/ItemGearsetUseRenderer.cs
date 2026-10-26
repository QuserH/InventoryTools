using System;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using InventoryTools.Localization;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemGearsetUseRenderer : ItemInfoRenderer<ItemGearsetSource>
{
    public ItemGearsetUseRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.Gearset;
    public override string SingularName => "套装";
    public override string HelpText => "该物品属于套装？";

    public override bool ShouldGroup => true;

    public override Func<ItemSource, (Type, uint)>? RelatedType => source =>
    {
        var asSource = AsSource(source);
        return (asSource.Gearset.GetType(), (uint)asSource.GearsetIndex);
    };

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        if (asSource.SetItems.Count > 1)
        {
            ImGui.Text("套装名称：" +  asSource.Gearset.Name);
            this.DrawItems("套装物品：", asSource.RelatedItems.First().Value);
        }
    };

    public override Func<ItemSource, string> GetName => source => "";
    public override Func<ItemSource, int> GetIcon => gearset => Icons.ArmorIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return "包含 " + string.Join(", ", asSource.SetItems.Select(c => c.NameString));
    };
}