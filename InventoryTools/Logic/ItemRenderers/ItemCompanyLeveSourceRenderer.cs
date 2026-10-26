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

public class ItemCompanyLeveSourceRenderer : ItemInfoRenderer<ItemCompanyLeveSource>
{
    public ItemCompanyLeveSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Source;
    public override ItemInfoType Type => ItemInfoType.CompanyLeve;
    public override string SingularName => "部队理符";
    public override string PluralName => "部队理符";
    public override string HelpText => "该物品是否通过部队理符获得？";
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
        ImGui.TextUnformatted("军票奖励：" + asSource.SealsRewarded);
        ImGui.TextUnformatted("允许消耗：" + leveRow.AllowanceCost);
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