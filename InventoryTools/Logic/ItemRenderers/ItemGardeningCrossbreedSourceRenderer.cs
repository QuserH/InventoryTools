using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Interface.Textures;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using InventoryTools.Localization;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemGardeningCrossbreedSourceRenderer : ItemInfoRenderer<ItemGardeningCrossbreedSource>
{
    private readonly ITextureProvider _textureProvider;
    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.GardeningCrossbreed;
    public override string SingularName => "园艺杂交";
    public override string HelpText => "该物品是否由两种种子杂交培育而成？";
    public override bool ShouldGroup => true;

    public ItemGardeningCrossbreedSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
        _textureProvider = textureProvider;
    }

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        ImGui.Text(string.Format("结果：{0}", asSource.SeedResult.NameString));
        ImGui.Text(string.Format("{0} + {1}", asSource.Seed1.NameString, asSource.Seed2.NameString));
    };
    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        return $"{asSource.SeedResult.NameString} - {asSource.Seed1.NameString} + {asSource.Seed2.NameString}";
    };

    public override Action<List<ItemSource>>? DrawTooltipGrouped => sources =>
    {
        var actualSources = AsSource(sources);
        ImGui.Text("杂交：");
        var chunkedSources = actualSources.OrderBy(c =>c.Seed1.NameString).Chunk(actualSources.Count / MaxColumns);
        using (var table = ImRaii.Table("CrossbreedTable", this.MaxColumns, ImGuiTableFlags.SizingStretchProp))
        {
            if (table)
            {
                ImGui.TableNextRow();
                foreach (var chunkedSource in chunkedSources)
                {
                    ImGui.TableNextColumn();
                    foreach (var source in chunkedSource)
                    {
                        ImGui.Image(_textureProvider.GetFromGameIcon(new GameIconLookup(source.Seed1.Icon)).GetWrapOrEmpty().Handle, new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
                        ImGui.SameLine();
                        ImGui.Text(string.Format("{0}", source.Seed1.NameString));
                        ImGui.SameLine();
                        ImGui.Text(" x ");
                        ImGui.SameLine();
                        ImGui.Image(_textureProvider.GetFromGameIcon(new GameIconLookup(source.Seed2.Icon)).GetWrapOrEmpty().Handle, new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
                        ImGui.SameLine();
                        ImGui.Text(string.Format("{0}", source.Seed2.NameString));
                    }
                }
            }
        }
        //
        // using var style = ImRaii.PushStyle(ImGuiStyleVar.CellPadding, new Vector2(5, 5));
        // using (var table = ImRaii.Table("CrossbreedTable", this.MaxColumns, ImGuiTableFlags.SizingStretchProp))
        // {
        //     if (table)
        //     {
        //         ImGui.TableNextRow();
        //         var count = 0;
        //         foreach (var groupedSource in actualSources.GroupBy(c => c.Seed1).OrderBy(c => c.Key.NameString))
        //         {
        //             ImGui.TableNextColumn();
        //             foreach (var source in groupedSource)
        //             {
        //                 ImGui.Text(LocalizationService.Format("{0} + {1}", source.Seed1.NameString, source.Seed2.NameString));
        //             }
        //             count++;
        //             if (count == this.MaxColumns)
        //             {
        //                 count = 0;
        //                 ImGui.TableNextRow();
        //             }
        //         }
        //     }
        // }
    };

    public override Func<ItemSource, int> GetIcon => _ => Icons.SeedBagIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return $"{asSource.SeedResult.NameString} - {asSource.Seed1.NameString} x {asSource.Seed2.NameString}";
    };
}

public class ItemGardeningCrossbreedSourceUseRenderer : ItemGardeningCrossbreedSourceRenderer
{
    public ItemGardeningCrossbreedSourceUseRenderer(ItemSheet itemSheet, MapSheet mapSheet,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, textureProvider, dalamudPluginInterface)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override string SingularName => "园艺杂交种子";
    public override string HelpText => "园艺培育时该物品是否为杂交的一部分？";

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return $"{asSource.SeedResult.NameString} - {asSource.Seed1.NameString} x {asSource.Seed2.NameString}";
    };
}