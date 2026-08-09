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
    public override string SingularName => LocalizationService.Ui("Interior Furniture");
    public override string HelpText => LocalizationService.Ui(LocalizationService.Ui("Can the item be placed inside houses?"));
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories => [ItemInfoRenderCategory.House];

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        ImGui.Text(LocalizationService.Format(LocalizationService.Ui("Category: {0}"), asSource.FurnitureCatalogItemList.ValueNullable?.Category.Value.Category.ExtractText() ?? "N/A"));
        ImGui.Text(LocalizationService.Format(LocalizationService.Ui("Patch Added: {0}"), asSource.FurnitureCatalogItemList.ValueNullable?.Patch.ToString() ?? "N/A"));
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        return source.Item.NameString;
    };
    public override Func<ItemSource, int> GetIcon => source => Icons.TableIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return LocalizationService.Ui("Can be placed inside a house.");
    };
}