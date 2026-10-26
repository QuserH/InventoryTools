using System;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using InventoryTools.Localization;

namespace InventoryTools.Logic.ItemRenderers;

public class ItemJobSoulCrystalUseRenderer : ItemInfoRenderer<ItemJobSoulCrystalUse>
{
    public ItemJobSoulCrystalUseRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.JobSoulCrystal;
    public override string SingularName => "特职水晶";
    public override string PluralName => "特职水晶";
    public override string HelpText => "装备后从基础职业转职为特职，或由特职使用？";
    public override bool ShouldGroup => false;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        if (asSource.ParentClassJob != null)
        {
            ImGui.Text("转换为" + asSource.ParentClassJob.Base.Name.ExtractText() + " 转换为 " + asSource.ClassJob.Base.Name.ExtractText());
        }
        else
        {
            ImGui.Text("提供" + asSource.ClassJob.Base.Name.ExtractText() + " 职业");
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = AsSource(source);
        return asSource.ClassJob.Base.Name.ExtractText();
    };

    public override Func<ItemSource, int> GetIcon => source =>
    {
        var asSource = AsSource(source);
        return asSource.ClassJob.Icon;
    };

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        if (asSource.ParentClassJob != null)
        {
            return "转换为" + asSource.ParentClassJob.Base.Name.ExtractText() + " 转换为 " + asSource.ClassJob.Base.Name.ExtractText();
        }

        return "提供" + asSource.ClassJob.Base.Name.ExtractText() + " 职业";
    };
}