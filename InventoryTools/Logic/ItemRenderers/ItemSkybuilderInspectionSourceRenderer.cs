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

public class ItemSkybuilderInspectionUseRenderer : ItemSkybuilderInspectionSourceRenderer
{
    public ItemSkybuilderInspectionUseRenderer(ItemSheet itemSheet, MapSheet mapSheet,
        GatheringItemSheet gatheringItemSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(itemSheet, mapSheet, gatheringItemSheet, textureProvider, dalamudPluginInterface)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override string HelpText => "该物品是否为穹顶皓天待鉴定物品的鉴定后形态？";
    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        ImGui.Text(string.Format("奖励：{0}", asSource.Item.NameString));
        ImGui.Text(string.Format("所需：{0}", asSource.InspectionData.AmountRequired));
    };
}

public class ItemSkybuilderInspectionSourceRenderer : ItemInfoRenderer<ItemSkybuilderInspectionSource>
{
    private readonly GatheringItemSheet _gatheringItemSheet;

    public ItemSkybuilderInspectionSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet,
        GatheringItemSheet gatheringItemSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
        _gatheringItemSheet = gatheringItemSheet;
    }
    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.SkybuilderInspection;
    public override string SingularName => "天穹街鉴定";
    public override string HelpText => "该物品是否可在穹顶皓天鉴定以转换为鉴定后形态？";
    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        ImGui.Text(string.Format("所需物品：{0}", asSource.CostItem?.NameString ?? LocalizationService.Ui(LocalizationService.Ui("Unknown Item"))));
        ImGui.Text(string.Format("所需数量：{0}", asSource.InspectionData.AmountRequired));
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        return asSource.Item.NameString;
    };
    public override Func<ItemSource, int> GetIcon => _ => Icons.SkybuildersScripIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return $"{asSource.CostItem?.NameString} x {asSource.InspectionData.AmountRequired}";
    };
}