using System.Collections.Generic;
using System.Linq;
using AllaganLib.Monitors.Interfaces;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib;
using Dalamud.Bindings.ImGui;
using InventoryTools.Localization;

namespace InventoryTools.Debuggers;

public class AchievementDebuggerPane : DebugLogPane
{
    private readonly IAchievementMonitorService _achievementMonitorService;

    public AchievementDebuggerPane(IAchievementMonitorService achievementMonitorService)
    {
        _achievementMonitorService = achievementMonitorService;
    }

    public override string Name => LocalizationService.Ui("Achievement Monitor");

    public override void SubscribeToEvents()
    {
    }

    public override void DrawInfo()
    {
        if (ImGui.CollapsingHeader(LocalizationService.Ui("Status")))
        {
            ImGui.TextUnformatted(LocalizationService.Format("Loaded: {0}", _achievementMonitorService.IsLoaded));
            ImGui.TextUnformatted($"Completed Achievement Count: {_achievementMonitorService.GetCompletedAchievementIds().Count}");
        }

        if (ImGui.CollapsingHeader(LocalizationService.Ui("Completed Achievements")))
        {
            var completed = _achievementMonitorService.GetCompletedAchievements();

            if (completed.Count == 0)
            {
                ImGui.TextUnformatted(LocalizationService.Ui("<none>"));
            }
            else
            {
                foreach (var rowRef in completed.OrderBy(r => r.RowId))
                {
                    var name = rowRef.ValueNullable?.Name.ToImGuiString() ?? $"<unknown name>";
                    ImGui.TextUnformatted(LocalizationService.Format("ID={0}, Name={1}", rowRef.RowId, name));
                }
            }
        }

        if (ImGui.CollapsingHeader(LocalizationService.Ui("Configuration")))
        {
            var config = _achievementMonitorService.Configuration;
            if (config == null)
            {
                ImGui.TextUnformatted(LocalizationService.Ui("<no configuration>"));
            }
            else
            {
                // Print configuration recursively
                Utils.PrintOutObject(config, 0, new List<string>());
            }
        }
    }
}