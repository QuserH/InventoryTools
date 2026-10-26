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

    public override string Name => "成就监控器";

    public override void SubscribeToEvents()
    {
    }

    public override void DrawInfo()
    {
        if (ImGui.CollapsingHeader("状态"))
        {
            ImGui.TextUnformatted(string.Format("已加载：{0}", _achievementMonitorService.IsLoaded));
            ImGui.TextUnformatted($"Completed Achievement Count: {_achievementMonitorService.GetCompletedAchievementIds().Count}");
        }

        if (ImGui.CollapsingHeader("已完成的成就"))
        {
            var completed = _achievementMonitorService.GetCompletedAchievements();

            if (completed.Count == 0)
            {
                ImGui.TextUnformatted("<无>");
            }
            else
            {
                foreach (var rowRef in completed.OrderBy(r => r.RowId))
                {
                    var name = rowRef.ValueNullable?.Name.ToImGuiString() ?? $"<unknown name>";
                    ImGui.TextUnformatted(string.Format("ID={0}，名称={1}", rowRef.RowId, name));
                }
            }
        }

        if (ImGui.CollapsingHeader("设置"))
        {
            var config = _achievementMonitorService.Configuration;
            if (config == null)
            {
                ImGui.TextUnformatted("<未配置>");
            }
            else
            {
                // Print configuration recursively
                Utils.PrintOutObject(config, 0, new List<string>());
            }
        }
    }
}