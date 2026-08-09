using System;
using System.Linq;
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

public class ItemGlamourReadySetSourceRenderer : ItemInfoRenderer<ItemGlamourReadySetSource>
{
    public ItemGlamourReadySetSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => ItemInfoType.GlamourReadySet;
    public override string SingularName => LocalizationService.Ui("Outfit Glamour Set");
    public override string HelpText => LocalizationService.Ui(LocalizationService.Ui("Is this item the combined form of a 'Outfit Glamour' set?"));

    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = AsSource(source);
        if (asSource.SetItems.Count > 1)
        {
            ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Set Items:")));
            using (ImRaii.PushIndent())
            {
                foreach (var item in asSource.SetItems)
                {
                    ImGui.Text(item.NameString);
                }
            }
        }
    };

    public override Func<ItemSource, string> GetName => source => "";
    public override Func<ItemSource, int> GetIcon => _ => Icons.MannequinIcon;

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        return LocalizationService.Ui("Contains ") + string.Join(", ", asSource.SetItems.Select(c => c.NameString));
    };
}