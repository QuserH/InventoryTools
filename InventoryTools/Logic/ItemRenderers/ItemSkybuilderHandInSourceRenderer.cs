using System;
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

public class ItemSkybuilderHandInSourceRenderer : ItemInfoRenderer<ItemSkybuilderHandInSource>
{
    private readonly GatheringItemSheet _gatheringItemSheet;

    public ItemSkybuilderHandInSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet,
        GatheringItemSheet gatheringItemSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
        _gatheringItemSheet = gatheringItemSheet;
    }
    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.SkybuilderHandIn;
    public override string SingularName => "天穹街交付";
    public override string HelpText => "该物品是否可在穹顶皓天兑换天穹票据？";
    public override bool ShouldGroup => false;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        var baseReward = asSource.HWDCrafterSupplyParams.BaseCollectableReward.Value;
        var midReward = asSource.HWDCrafterSupplyParams.MidCollectableReward.Value;
        var highReward = asSource.HWDCrafterSupplyParams.HighCollectableReward.Value;
        ImGui.Text("等级：" + asSource.Level);
        ImGui.Text("最高等级：" + asSource.LevelMax);

        ImGui.Text("奖励：");
        using (ImRaii.PushIndent())
        {
            ImGui.Text("经验：" + baseReward.ExpReward + "/" + midReward.ExpReward + "/" + highReward.ExpReward);
            ImGui.Text("票据：" + baseReward.ScriptRewardAmount + "/" + midReward.ScriptRewardAmount + "/" +
                       highReward.ScriptRewardAmount);
            ImGui.Text("点数：" + baseReward.Points + "/" + midReward.Points + "/" + highReward.Points);
        }
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
        var baseReward = asSource.HWDCrafterSupplyParams.BaseCollectableReward.Value;
        var midReward = asSource.HWDCrafterSupplyParams.MidCollectableReward.Value;
        var highReward = asSource.HWDCrafterSupplyParams.HighCollectableReward.Value;
        return $"Levels {asSource.Level} - {asSource.LevelMax} ({baseReward.ExpReward} xp, {midReward.ExpReward} xp, {highReward.ExpReward} xp), ({baseReward.ScriptRewardAmount} script, {midReward.ScriptRewardAmount} script, {highReward.ScriptRewardAmount} script), ({baseReward.Points} points, {midReward.Points} points, {highReward.Points} points)";
    };
}