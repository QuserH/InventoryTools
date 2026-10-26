using System.Numerics;
using CriticalCommonLib.Services;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Compendium.Models;
using InventoryTools.Mediator;
using InventoryTools.Localization;

namespace InventoryTools.Compendium.MenuProviders;

public class MapLinkEntryMenuProvider(MediatorService mediatorService, IChatUtilities chatUtilities) : MenuProvider<MapLinkEntry>(mediatorService)
{
    public override void DrawMenu(MapLinkEntry item)
    {
        ImGui.Text(item.Name);
        ImGui.Text(item.Subtitle);
        ImGui.Separator();
        if (ImGui.Selectable("在地图上显示"))
        {
            //TODO: add map location message
            chatUtilities.PrintFullMapLink(item.Location);
        }

        if (ImGui.Selectable("传送"))
        {
            MediatorService.Publish(new RequestTeleportToMapMessage(item.Location.Map.RowId, new Vector2((float)item.Location.MapX, (float)item.Location.MapY)));
            chatUtilities.PrintFullMapLink(item.Location);
        }
    }

    public override string PopupName => "MapLinkEntry";
}