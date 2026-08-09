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

public class ItemGcSupplyDutySourceRenderer : ItemInfoRenderer<ItemGCSupplyDutySource>
{
    public ItemGcSupplyDutySourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.GCDailySupply;
    public override string SingularName => LocalizationService.Ui("Grand Company Supply & Provisioning");
    public override string HelpText => LocalizationService.Ui(LocalizationService.Ui("Can the item be handed in for 'Supply & Provisioning' at your grand company?"));
    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        var rewardRow = asSource.DailySupplyRewardRow;
        if (rewardRow != null)
        {
            var baseReward = rewardRow.Base.ExperienceSupply;
            var sealsSupply = rewardRow.Base.SealsSupply;
            ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Level: ")) + asSource.GCSupplyDutyRow.RowId);
            ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Exp: ")) + baseReward);
            ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Seals: ")) + sealsSupply);
        }
        else
        {
            ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Unknown rewards")));
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        var rewardRow = asSource.DailySupplyRewardRow;
        return rewardRow != null ? asSource.GCSupplyDutyRow.RowId.ToString() : "";
    };
    public override Func<ItemSource, int> GetIcon => _ => Icons.FlameSealIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        var rewardRow = asSource.DailySupplyRewardRow;
        if (rewardRow != null)
        {
            var baseReward = rewardRow.Base.ExperienceSupply;
            var sealsSupply = rewardRow.Base.SealsSupply;
            return $"Level {asSource.GCSupplyDutyRow.RowId} ({baseReward} xp, {sealsSupply} seals)";
        }
        else
        {
            return "Unknown rewards";
        }
    };
}