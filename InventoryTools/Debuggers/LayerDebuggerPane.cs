using AllaganLib.Shared.Interfaces;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using InventoryTools.Localization;

namespace InventoryTools.Debuggers;

public class LayerDebuggerPane : IDebugPane
{
    public string Name => LocalizationService.Ui("Layer Debugger");
    public unsafe void Draw()
    {
        var activeLayout = LayoutWorld.Instance()->ActiveLayout;
        if (activeLayout != null)
        {
            ImGui.TextUnformatted($"Level ID: {activeLayout->LevelId}");
            ImGui.TextUnformatted($"ID: {activeLayout->Id}");
            ImGui.TextUnformatted($"Type: {activeLayout->Type}");
            ImGui.TextUnformatted($"Resource Strings: {activeLayout->Type}");
            foreach (var resourcePath in activeLayout->ResourcePaths.Strings)
            {
                if (resourcePath.Value != null)
                {
                    ImGui.TextUnformatted($"{resourcePath.Value->DataString}");
                }
            }
            ImGui.TextUnformatted($"Layers:");
            foreach (var layer in activeLayout->Layers)
            {
                ImGui.TextUnformatted(LocalizationService.Format("{0}", layer.Item1));
                var pointer = layer.Item2.Value;
                if (pointer != null)
                {
                    ImGui.TextUnformatted($"Layer ID: " + pointer->Id);
                    ImGui.TextUnformatted($"Layer Group ID: " + pointer->LayerGroupId);
                    ImGui.TextUnformatted($"Festival ID: " + pointer->FestivalId);
                }
            }
        }
    }
}