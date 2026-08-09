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
    public override string SingularName => LocalizationService.Ui("Sky Builder Hand In");
    public override string HelpText => LocalizationService.Ui(LocalizationService.Ui("Can the item be handed in at the firmament for skybuilders' scrip?"));
    public override bool ShouldGroup => false;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        var baseReward = asSource.HWDCrafterSupplyParams.BaseCollectableReward.Value;
        var midReward = asSource.HWDCrafterSupplyParams.MidCollectableReward.Value;
        var highReward = asSource.HWDCrafterSupplyParams.HighCollectableReward.Value;
        ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Level: ")) + asSource.Level);
        ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Max Level: ")) + asSource.LevelMax);

        ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Rewards:")));
        using (ImRaii.PushIndent())
        {
            ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Exp: ")) + baseReward.ExpReward + "/" + midReward.ExpReward + "/" + highReward.ExpReward);
            ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Script: ")) + baseReward.ScriptRewardAmount + "/" + midReward.ScriptRewardAmount + "/" +
                       highReward.ScriptRewardAmount);
            ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Points: ")) + baseReward.Points + "/" + midReward.Points + "/" + highReward.Points);
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