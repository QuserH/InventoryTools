using System;
using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using InventoryTools.Localization;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemFurnitureSourceRenderer : ItemInfoRenderer<ItemFurnitureSource>
{
    public ItemFurnitureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.FurnitureItem;
    public override string SingularName => "室内家具";
    public override string HelpText => "该物品是否可放入室内？";
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories => [ItemInfoRenderCategory.House];

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        ImGui.Text(string.Format("分类：{0}", asSource.FurnitureCatalogItemList.ValueNullable?.Category.Value.Category.ExtractText() ?? "不适用"));
        ImGui.Text(string.Format("加入版本：{0}", asSource.FurnitureCatalogItemList.ValueNullable?.Patch.ToString() ?? "不适用"));
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        return source.Item.NameString;
    };
    public override Func<ItemSource, int> GetIcon => source => Icons.TableIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return "可放置在房屋内部。";
    };
}