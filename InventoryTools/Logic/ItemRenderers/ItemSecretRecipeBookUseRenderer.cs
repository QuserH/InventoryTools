using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using InventoryTools.Localization;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemSecretRecipeBookUseRenderer : ItemInfoRenderer<ItemSecretRecipeBookUse>
{
    public ItemSecretRecipeBookUseRenderer(ItemSheet itemSheet, MapSheet mapSheet,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface)
        : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.SecretRecipeBook;
    public override string SingularName => "秘籍";
    public override string? PluralName => "秘籍";
    public override string HelpText => "该物品是否用于解锁秘籍？";
    public override bool ShouldGroup => false;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Crafting];

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        var bookName = asSource.SecretRecipeBook.ValueNullable?.Name.ExtractText();
        if (!string.IsNullOrEmpty(bookName))
        {
            ImGui.Text("解锁：" + bookName);
        }

        if (asSource.RelatedItems.Count > 0)
        {
            this.DrawItems("解锁的配方：", asSource.RelatedItems.First().Value);
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        return asSource.SecretRecipeBook.ValueNullable?.Name.ExtractText() ?? asSource.Item.NameString;
    };

    public override Func<ItemSource, int> GetIcon => source =>
    {
        var asSource = AsSource(source);
        return asSource.Item.Base.Icon;
    };

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return $"Unlocks {asSource.Recipes.Count} recipes";
    };
}