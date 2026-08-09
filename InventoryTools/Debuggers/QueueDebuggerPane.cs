using System.Collections.Generic;
using AllaganLib.Shared.Interfaces;
using AllaganLib.Shared.Services;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using InventoryTools.Localization;

namespace InventoryTools.Debuggers;

public class QueueDebuggerPane : IDebugPane
{
    private readonly BackgroundTaskCollector _backgroundTaskCollector;

    public QueueDebuggerPane(BackgroundTaskCollector backgroundTaskCollector)
    {
        _backgroundTaskCollector = backgroundTaskCollector;
    }

    public void Draw()
    {
        foreach (var taskQueue in _backgroundTaskCollector.BackgroundTaskQueues)
        {
            using (ImRaii.Group())
            {
                ImGui.Text(taskQueue.QueueName);
                ImGui.SameLine();
                ImGui.Text(taskQueue.QueueCount.ToString());
            }
        }
    }

    public string Name => LocalizationService.Ui("Queue Debugger");
}