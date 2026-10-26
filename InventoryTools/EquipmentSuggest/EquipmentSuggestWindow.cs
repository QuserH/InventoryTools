using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic;
using InventoryTools.Logic.Settings;
using InventoryTools.Mediator;
using InventoryTools.Services;
using InventoryTools.Ui;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using InventoryTools.Localization;

namespace InventoryTools.EquipmentSuggest;

public class EquipmentSuggestWindow : GenericWindow, IMenuWindow
{
    private readonly InventoryToolsConfiguration _configuration;
    private readonly Lazy<EquipmentSuggestGrid> _equipmentSuggestGrid;
    private readonly EquipmentSuggestClassJobFormField _classJobField;
    private readonly EquipmentSuggestSourceTypeField _sourceTypeField;
    private readonly EquipmentSuggestExcludeSourceTypeField _excludeSourceTypeField;
    private readonly EquipmentSuggestLevelFormField _levelField;
    private readonly EquipmentSuggestConfig _config;
    private readonly EquipmentSuggestSourceTypeField _typeField;
    private readonly IPluginLog _pluginLog;
    private readonly EquipmentSuggestViewModeSetting _viewModeSetting;
    private readonly Lazy<IEnumerable<IMenuWindow>> _menuWindows;
    private readonly EquipmentSuggestFilterStatsField _statsField;
    private readonly EquipmentSuggestModeSetting _modeSetting;
    private readonly EquipmentSuggestToolModeCategorySetting _toolModeCategorySetting;
    private readonly EquipmentSuggestService _equipmentSuggestService;
    private readonly ICharacterMonitor _characterMonitor;
    private Task? _currentTask;

    public EquipmentSuggestWindow(ILogger<EquipmentSuggestWindow> logger, MediatorService mediator,
        ImGuiService imGuiService, InventoryToolsConfiguration configuration,
        Lazy<EquipmentSuggestGrid> equipmentSuggestGrid, EquipmentSuggestClassJobFormField classJobField,
        EquipmentSuggestSourceTypeField sourceTypeField, EquipmentSuggestExcludeSourceTypeField excludeSourceTypeField,
        EquipmentSuggestLevelFormField levelField, EquipmentSuggestConfig config, EquipmentSuggestSourceTypeField typeField,
        IPluginLog pluginLog, EquipmentSuggestViewModeSetting viewModeSetting, Lazy<IEnumerable<IMenuWindow>> menuWindows,
        EquipmentSuggestFilterStatsField statsField, EquipmentSuggestModeSetting modeSetting, EquipmentSuggestToolModeCategorySetting toolModeCategorySetting,
        EquipmentSuggestService equipmentSuggestService,
        ICharacterMonitor characterMonitor) : base(logger, mediator, imGuiService, configuration,
        "装备推荐")
    {
        _configuration = configuration;
        _equipmentSuggestGrid = equipmentSuggestGrid;
        _classJobField = classJobField;
        _sourceTypeField = sourceTypeField;
        _excludeSourceTypeField = excludeSourceTypeField;
        _levelField = levelField;
        _config = config;
        _typeField = typeField;
        _pluginLog = pluginLog;
        _viewModeSetting = viewModeSetting;
        _menuWindows = menuWindows;
        _statsField = statsField;
        _modeSetting = modeSetting;
        _toolModeCategorySetting = toolModeCategorySetting;
        _equipmentSuggestService = equipmentSuggestService;
        _characterMonitor = characterMonitor;
        this.Flags = ImGuiWindowFlags.MenuBar;
    }

    private void DrawMenuBar()
    {
        using (var menuBar = ImRaii.MenuBar())
        {
            if (menuBar)
            {
                using (var menu = ImRaii.Menu("文件"))
                {
                    if (menu)
                    {
                        if (ImGui.MenuItem("设置"))
                        {
                            MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWindow)));
                        }

                        if (ImGui.MenuItem("更新日志"))
                        {
                            MediatorService.Publish(new OpenGenericWindowMessage(typeof(ChangelogWindow)));
                        }

                        if (ImGui.MenuItem("帮助"))
                        {
                            MediatorService.Publish(new OpenGenericWindowMessage(typeof(HelpWindow)));
                        }

                        if (ImGui.MenuItem("启用详细日志", "",
                                this._pluginLog.MinimumLogLevel == LogEventLevel.Verbose))
                        {
                            if (this._pluginLog.MinimumLogLevel == LogEventLevel.Verbose)
                            {
                                this._pluginLog.MinimumLogLevel = LogEventLevel.Debug;
                            }
                            else
                            {
                                this._pluginLog.MinimumLogLevel = LogEventLevel.Verbose;
                            }
                        }

                        if (ImGui.MenuItem("生成支持信息包"))
                        {
                            MediatorService.Publish(new OpenGenericWindowMessage(typeof(SupportDumpWindow)));
                        }

                        if (ImGui.MenuItem("报告问题"))
                        {
                            "https://github.com/Critical-Impact/InventoryTools".OpenBrowser();
                        }

                        if (ImGui.MenuItem("Ko-Fi"))
                        {
                            "https://ko-fi.com/critical_impact".OpenBrowser();
                        }

                        if (ImGui.MenuItem("关闭"))
                        {
                            this.IsOpen = false;
                        }
                    }
                }

                using (var menu = ImRaii.Menu("模式"))
                {
                    if (menu)
                    {
                        if (ImGui.MenuItem("职业", "",
                                _modeSetting.CurrentValue(_configuration) == EquipmentSuggestMode.Class))
                        {
                            _modeSetting.UpdateFilterConfiguration(_configuration, EquipmentSuggestMode.Class);
                        }

                        if (ImGui.MenuItem("工具/武器", "",
                                _modeSetting.CurrentValue(_configuration) == EquipmentSuggestMode.Tool))
                        {
                            _modeSetting.UpdateFilterConfiguration(_configuration, EquipmentSuggestMode.Tool);
                        }
                    }
                }

                using (var menu = ImRaii.Menu("视图"))
                {
                    if (menu)
                    {
                        if (ImGui.MenuItem("普通", "",
                                _viewModeSetting.CurrentValue(_configuration) == EquipmentSuggestViewMode.Normal))
                        {
                            _viewModeSetting.UpdateFilterConfiguration(_configuration, EquipmentSuggestViewMode.Normal);
                        }

                        if (ImGui.MenuItem("展开", "",
                                _viewModeSetting.CurrentValue(_configuration) == EquipmentSuggestViewMode.Expanded))
                        {
                            _viewModeSetting.UpdateFilterConfiguration(_configuration,
                                EquipmentSuggestViewMode.Expanded);
                        }

                        if (ImGui.MenuItem("紧凑", "",
                                _viewModeSetting.CurrentValue(_configuration) == EquipmentSuggestViewMode.Compact))
                        {
                            _viewModeSetting.UpdateFilterConfiguration(_configuration,
                                EquipmentSuggestViewMode.Compact);
                        }
                    }
                }

                using (var menu = ImRaii.Menu("窗口"))
                {
                    if (menu)
                    {
                        foreach (var window in _menuWindows.Value)
                        {
                            if (ImGui.MenuItem(window.GenericName))
                            {
                                MediatorService.Publish(new OpenGenericWindowMessage(window.GetType()));
                            }
                        }
                    }
                }
            }
        }
    }


    public override void DrawWindow()
    {
        if (ImGui.GetWindowPos() != CurrentPosition)
        {
            CurrentPosition = ImGui.GetWindowPos();
        }

        if (ImGui.GetWindowPos() == Position)
        {
            Position = null;
        }

        if (_currentTask is { IsCompleted: true })
        {
            _currentTask = null;
        }
        DrawMenuBar();
        DrawTopFilters();
        DrawGrid();
        DrawBottomBar();
    }

    private void DrawGrid()
    {
        using (var contentChild = ImRaii.Child("Content", new Vector2(0, -44) * ImGui.GetIO().FontGlobalScale, true))
        {
            if (contentChild)
            {
                MediatorService.Publish(_equipmentSuggestGrid.Value.Draw(_config, new Vector2(0, 0)));
            }
        }
    }

    private void DrawBottomBar()
    {
        using (var bottomBarChild = ImRaii.Child("BottomBar", new Vector2(0, 0), true, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
        {
            if (bottomBarChild.Success)
            {
                MediatorService.Publish(_equipmentSuggestGrid.Value.DrawFooter(_config, new Vector2(0, 0)));
            }
        }
    }

    private void DrawTopFilters()
    {
        using (var topBar = ImRaii.Child("TopBar", new Vector2(0, 70) * ImGui.GetIO().FontGlobalScale, true))
        {
            if (topBar)
            {
                using (var child = ImRaii.Child("levelField", new Vector2(100, 50) * ImGui.GetIO().FontGlobalScale, false, ImGuiWindowFlags.NoScrollbar))
                {
                    if (child)
                    {
                        _levelField.Draw(_config);
                    }
                }

                if (_modeSetting.CurrentValue(_configuration) == EquipmentSuggestMode.Class)
                {
                    ImGui.SameLine();
                    using (var child = ImRaii.Child("classJobField",
                               new Vector2(100, 50) * ImGui.GetIO().FontGlobalScale, false,
                               ImGuiWindowFlags.NoScrollbar))
                    {
                        if (child)
                        {
                            _classJobField.Draw(_config);
                        }
                    }
                }
                else if (_modeSetting.CurrentValue(_configuration) == EquipmentSuggestMode.Tool)
                {
                    ImGui.SameLine();
                    using (var child = ImRaii.Child("classJobField",
                               new Vector2(100, 50) * ImGui.GetIO().FontGlobalScale, false,
                               ImGuiWindowFlags.NoScrollbar))
                    {
                        if (child)
                        {
                            _toolModeCategorySetting.Draw(_config);
                        }
                    }
                }

                ImGui.SameLine();
                using (var child = ImRaii.Child("sourceTypeField", new Vector2(150, 50) * ImGui.GetIO().FontGlobalScale, false, ImGuiWindowFlags.NoScrollbar))
                {
                    if (child)
                    {
                        _sourceTypeField.Draw(_config);
                    }
                }

                ImGui.SameLine();
                using (var child = ImRaii.Child("excludeSourceTypeField", new Vector2(150, 50) * ImGui.GetIO().FontGlobalScale, false, ImGuiWindowFlags.NoScrollbar))
                {
                    if (child)
                    {
                        _excludeSourceTypeField.Draw(_config);
                    }
                }

                ImGui.SameLine();
                using (var child = ImRaii.Child("statsField", new Vector2(100, 50) * ImGui.GetIO().FontGlobalScale, false, ImGuiWindowFlags.NoScrollbar))
                {
                    if (child)
                    {
                        _statsField.Draw(_config);
                    }
                }

                ImGui.SameLine();
                var text = _modeSetting.CurrentValue(_configuration) == EquipmentSuggestMode.Tool ? "使用当前等级" : "使用当前职业/等级";
                var textSize = ImGui.CalcTextSize(text).X + ImGui.GetStyle().ItemSpacing.X * 2;
                var childSize = new Vector2(textSize, 50) * ImGui.GetIO().FontGlobalScale;
                using (var child = ImRaii.Child("5", childSize, false, ImGuiWindowFlags.NoScrollbar))
                {
                    if (child)
                    {
                        var activeCharacter = _characterMonitor.ActiveCharacter;

                        using var disabled = ImRaii.Disabled(activeCharacter == null);
                        ImGui.SetNextItemWidth(400);
                        using var color = ImRaii.PushColor(ImGuiCol.Text, new Vector4(0, 0, 0, 0));
                        ImGui.LabelText("5Label", text);
                        color.Pop();
                        if (ImGui.Button(text) && activeCharacter != null)
                        {
                            _equipmentSuggestService.UseCurrentClassLevel();
                        }
                    }
                }

                ImGui.SameLine();
                text = "自动选择最佳物品";
                textSize = ImGui.CalcTextSize(text).X + ImGui.GetStyle().ItemSpacing.X * 2;
                childSize = new Vector2(textSize, 50) * ImGui.GetIO().FontGlobalScale;
                using (var child = ImRaii.Child("6", childSize, false, ImGuiWindowFlags.NoScrollbar))
                {
                    if (child)
                    {
                        var classJob = _classJobField.CurrentValue(_config);
                        ImGui.SetNextItemWidth(400);
                        using var color = ImRaii.PushColor(ImGuiCol.Text, new Vector4(0, 0, 0, 0));
                        ImGui.LabelText("6Label", text);
                        color.Pop();
                        using var disabled = ImRaii.Disabled(classJob == 0 && _modeSetting.CurrentValue(_configuration) == EquipmentSuggestMode.Class);
                        if (ImGui.Button(text))
                        {
                            if (_currentTask == null || _currentTask.IsCompleted)
                            {
                                _currentTask = Task.Run(() => _equipmentSuggestService.SelectHighestILvl());
                            }
                        }

                        if (ImGui.IsItemHovered())
                        {
                            using (ImRaii.Tooltip())
                            {
                                ImGui.Text(
                                    "点击此项将选取品级最高的物品，同时考虑选定职业/物品的相关属性。");
                            }
                        }
                    }
                }
                ImGui.SameLine();
                childSize = new Vector2(100, 50) * ImGui.GetIO().FontGlobalScale;
                using (var child = ImRaii.Child("Spin", childSize, false, ImGuiWindowFlags.NoScrollbar))
                {
                    if (child)
                    {
                        using var color = ImRaii.PushColor(ImGuiCol.Text, new Vector4(0, 0, 0, 0));
                        ImGui.LabelText("SpinLabel", text);
                        if (_equipmentSuggestGrid.Value.IsLoading || _currentTask != null)
                        {
                            ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 5 * ImGui.GetIO().FontGlobalScale);
                            float nextDot = 3.0f;
                            ImGuiService.SpinnerDots("Loading", ref nextDot, 7, 1);
                        }
                    }
                }
            }
        }
    }

    public override void Invalidate()
    {

    }

    public override FilterConfiguration? SelectedConfiguration { get; } = null;
    public override string GenericKey { get; } = "EquipmentSuggest";
    public override string GenericName { get; } = "装备推荐";
    public override bool DestroyOnClose { get; } = true;
    public override bool SaveState { get; } = true;
    public override Vector2? DefaultSize { get; } = new Vector2(800, 500);
    public override Vector2? MaxSize { get; } = null;
    public override Vector2? MinSize { get; } = null;
    public override void Initialize()
    {
    }
}