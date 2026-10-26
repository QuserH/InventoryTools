using AllaganLib.Shared.Interfaces;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.LayoutEngine;
using InventoryTools.Localization;

namespace InventoryTools.Debuggers;

public class LayerDebuggerPane : IDebugPane
{
    public string Name => "层级调试器";
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
                ImGui.TextUnformatted(string.Format("{0}", layer.Item1));
                var pointer = layer.Item2.Value;
                if (pointer != null)
                {
                    ImGui.TextUnformatted($"图层 ID：" + pointer->Id);
                    ImGui.TextUnformatted($"图层组 ID：" + pointer->LayerGroupId);
                    ImGui.TextUnformatted($"节日 ID：" + pointer->FestivalId);
                }
            }
        }
    }
}