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
using InventoryTools.Extensions;
using InventoryTools.Localization;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemCraftLeveSourceRenderer : ItemInfoRenderer<ItemCraftLeveSource>
{
    private readonly ITextureProvider _textureProvider;
    private readonly ItemSheet _itemSheet;
    private readonly MapSheet _mapSheet;
    private readonly IDalamudPluginInterface _pluginInterface;

    public ItemCraftLeveSourceRenderer(ITextureProvider textureProvider, ItemSheet itemSheet, MapSheet mapSheet, IDalamudPluginInterface pluginInterface) : base(textureProvider, pluginInterface, itemSheet, mapSheet)
    {
        _textureProvider = textureProvider;
        _itemSheet = itemSheet;
        _mapSheet = mapSheet;
        _pluginInterface = pluginInterface;
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.CraftLeve;
    public override string SingularName => "制作理符";
    public override string PluralName => "制作理符";
    public override string HelpText => "该物品来自制作理符？";
    public override bool ShouldGroup => true;
    public override IReadOnlyList<ItemInfoRenderCategory> Categories => [ItemInfoRenderCategory.Leve];

    public override Func<ItemSource, (Type, uint)>? RelatedType => source =>
    {
        var asSource = AsSource(source);
        return (asSource.Leve.RowType, asSource.Leve.RowId);
    };

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        var leveRow = asSource.Leve.Value;

        ImGui.TextUnformatted("理符：" + leveRow.Name.ExtractText());
        ImGui.TextUnformatted("职业：" + leveRow.ClassJobCategory.Value.Name.ExtractText());
        ImGui.TextUnformatted("经验奖励：" + asSource.ExpReward);
        ImGui.TextUnformatted("允许消耗：" + leveRow.AllowanceCost);
        ImGui.TextUnformatted("掉落几率：" + asSource.LeveRewardItem.Value.ProbabilityPercent[asSource.RewardItemIndex] + "%");
        DrawItems("可能的奖励物品：", asSource.RewardItems);
        DrawMaps(source);
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        var leveRow = asSource.Leve.Value;
        return leveRow.Name.ExtractText();
    };
    public override Func<ItemSource, int> GetIcon => _ => Icons.LeveIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        var leveRow = asSource.Leve.Value;
        return
            $"{leveRow.Name.ExtractText()} ({leveRow.ClassJobCategory.Value.Name.ExtractText()}) ({leveRow.ExpReward} xp) ({leveRow.AllowanceCost} allowances)";
    };
}