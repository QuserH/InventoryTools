using CriticalCommonLib.Services;
using DalaMock.Shared.Interfaces;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using OtterGui;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TrackMobSpawnSetting : BooleanSetting
{
    private readonly IFileDialogManager _fileDialogManager;
    private readonly IMobTracker _mobTracker;

    public TrackMobSpawnSetting(ILogger<TrackMobSpawnSetting> logger, ImGuiService imGuiService, IFileDialogManager fileDialogManager, IMobTracker mobTracker) : base(logger, imGuiService)
    {
        _fileDialogManager = fileDialogManager;
        _mobTracker = mobTracker;
    }
    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TrackMobSpawns;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.TrackMobSpawns = newValue;
    }

    public override string Key { get; set; } = "TrackMobSpawns";
    public override string Name { get; set; } = "追踪怪物刷新";

    public override string HelpText { get; set; } =
        "插件是否在移动时追踪怪物刷新位置。这些数据目前尚未被插件使用，但收集足够后，可点击复选框旁的按钮导出包含这些位置的文件。若你上传这些 CSV 并通过反馈发送链接，我可以用这些刷新数据为所有人提供准确的怪物刷新位置。";

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.MobSpawnTracker;

    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;

    public override void Draw(InventoryToolsConfiguration configuration, string? customName, bool? disableReset,
        bool? disableColouring)
    {
        base.Draw(configuration, null, null, null);
        if (configuration.TrackMobSpawns)
        {
            ImGui.SameLine();
            if (ImGui.Button("导出 CSV"))
            {
                _fileDialogManager.SaveFileDialog("保存为 CSV", "*.csv", "mob_spawns.csv", ".csv",
                    (b, s) => { SaveMobSpawns(b, s); }, null, true);
            }

            ImGuiUtil.HoverTooltip("导出包含怪物刷新 ID 及其位置的 CSV 文件。");
        }
    }

    private void SaveMobSpawns(bool b, string s)
    {
        if (b)
        {
            var entries = _mobTracker.GetEntries();
            _mobTracker.SaveCsv(s, entries);
        }
    }
    public override string Version => "1.7.0.0";
}