using System;
using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using InventoryTools.Localization;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemCraftSoulCrystalUseRenderer : ItemInfoRenderer<ItemCraftSoulCrystalUse>
{
    public ItemCraftSoulCrystalUseRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.CraftSoulCrystal;
    public override string SingularName => "制作特职水晶";
    public override string PluralName => "制作特职水晶";
    public override string HelpText => "该特职水晶是否用于专精制作职业？";
    public override bool ShouldGroup => false;

    public override IReadOnlyList<ItemInfoRenderCategory> Categories =>
        [ItemInfoRenderCategory.Crafting];

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        ImGui.Text("装备以特化为" + asSource.ClassJob.Base.Name.ExtractText());
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        return asSource.ClassJob.Base.Name.ExtractText();
    };

    public override Func<ItemSource, int> GetIcon => source =>
    {
        var asSource = AsSource(source);
        return asSource.ClassJob.Icon;
    };

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return "装备以特化为" + asSource.ClassJob.Base.Name.ExtractText();
    };
}