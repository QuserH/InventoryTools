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

public class ItemExteriorFurnitureSourceRenderer : ItemInfoRenderer<ItemExteriorFurnitureSource>
{
    public ItemExteriorFurnitureSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.ExteriorFurnitureItem;
    public override string SingularName => LocalizationService.Ui("Exterior Furniture");
    public override string HelpText => LocalizationService.Ui(LocalizationService.Ui("Can the item be placed outside houses?"));
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory>? Categories => [ItemInfoRenderCategory.House];

    public override Action<ItemSource> DrawTooltip => source =>
    {
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        return source.Item.NameString;
    };
    public override Func<ItemSource, int> GetIcon => source => Icons.TableIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return LocalizationService.Ui("Can be placed outside a house.");
    };
}