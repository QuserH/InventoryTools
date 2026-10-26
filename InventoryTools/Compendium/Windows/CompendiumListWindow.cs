using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using AllaganLib.Shared.Extensions;
using Autofac.Features.OwnedInstances;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Models;
using InventoryTools.Compendium.Services;
using InventoryTools.Logic;
using InventoryTools.Mediator;
using InventoryTools.Services;
using InventoryTools.Ui;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using InventoryTools.Localization;

namespace InventoryTools.Compendium.Windows;

public class CompendiumListWindow : CompendiumWindow
{
    private readonly WindowState _windowState;
    private readonly CompendiumSectionStateService _sectionStateService;
    private SectionState? _sectionState;
    private Task<SectionState>? _sectionStateTask;
    private readonly ICompendiumType _compendiumType;
    private readonly IPluginLog _pluginLog;
    private readonly IEnumerable<IMenuWindow> _menuWindows;
    private readonly IEnumerable<ICompendiumType> _compendiumTypes;
    private readonly Lazy<ICompendiumTable<WindowState, MessageBase>> _table;
    private ICompendiumGrouping? _compendiumGrouping = null;
    private List<KeyValuePair<object, string>>? _compendiumTabs = null;
    private object? _currentTab = null;
    private bool _defaultGroupSet = false;

    public delegate Owned<CompendiumListWindow> Factory(ICompendiumType compendiumType);

    public CompendiumListWindow(ILogger<CompendiumListWindow> logger, WindowState windowState, CompendiumSectionStateService sectionStateService, MediatorService mediator, ImGuiService imGuiService, InventoryToolsConfiguration configuration, ICompendiumType compendiumType, IPluginLog pluginLog, IEnumerable<IMenuWindow> menuWindows, IEnumerable<ICompendiumType> compendiumTypes) : base(logger, mediator, imGuiService, configuration, compendiumType.Plural + " 窗口")
    {
        _windowState = windowState;
        _sectionStateService = sectionStateService;
        _compendiumType = compendiumType;
        _pluginLog = pluginLog;
        _menuWindows = menuWindows;
        _compendiumTypes = compendiumTypes.Where(c => c.ShowInListing).OrderBy(c => c.Plural);
        _table = new Lazy<ICompendiumTable<WindowState, MessageBase>>(
            compendiumType.BuildTable,
            LazyThreadSafetyMode.PublicationOnly);
        Flags = ImGuiWindowFlags.MenuBar;
    }

    private List<KeyValuePair<object, string>>? GetTabs(ICompendiumGrouping compendiumGrouping)
    {
        return _compendiumType.GetGroups(compendiumGrouping)?.OrderBy(k => k.Value).ToList();
    }

    private string? GetSavedGrouping()
    {
        return ((AllaganLib.Interface.FormFields.IConfigurable<string?>)_sectionState!).Get("grouping");
    }

    private void SetSavedGrouping(string? newGrouping)
    {
        _sectionState?.Set("grouping", newGrouping);
    }

    public override void DrawWindow()
    {
        if (_sectionState == null)
        {
            if (_sectionStateTask == null)
            {
                _sectionStateTask = _sectionStateService.GetState(_compendiumType, CompendiumSectionType.List);
            }

            if (_sectionStateTask.IsCompleted)
            {
                _sectionState = _sectionStateTask.Result;
                _sectionStateTask = null;
            }

            if (_sectionState == null)
            {
                return;
            }
        }

        DrawMenuBar();

        if (!_defaultGroupSet)
        {
            _defaultGroupSet = true;
            var savedGrouping = GetSavedGrouping();
            var groupings = _compendiumType.GetGroupings();
            if (groupings != null)
            {
                if (savedGrouping != null)
                {
                    _compendiumGrouping = groupings.FirstOrDefault(c => c.Key == savedGrouping);
                }

                var defaultGrouping = _compendiumType.GetDefaultGrouping();
                if (_compendiumGrouping == null && defaultGrouping != string.Empty)
                {
                    _compendiumGrouping = groupings.FirstOrDefault(c => c.Key == defaultGrouping);
                }
            }
        }

        if (_compendiumGrouping == null)
        {
            MediatorService.Publish(_table.Value.Draw(_windowState, new Vector2(0, 0)));
        }
        else
        {
            using (var tabBar = ImRaii.TabBar(_compendiumGrouping.Key, ImGuiTabBarFlags.FittingPolicyScroll | ImGuiTabBarFlags.ListPopupButton))
            {
                if (tabBar)
                {
                    _compendiumTabs ??= GetTabs(_compendiumGrouping);
                    if (_compendiumTabs == null)
                    {
                        MediatorService.Publish(_table.Value.Draw(_windowState, new Vector2(0, 0)));
                    }
                    else
                    {
                        foreach (var tab in _compendiumTabs)
                        {
                            using (var tabItem = ImRaii.TabItem(tab.Value, ImGuiTabItemFlags.NoReorder))
                            {
                                if (tabItem)
                                {
                                    if (_currentTab == null || _currentTab != tab.Key)
                                    {
                                        _currentTab = tab.Key;
                                        _table.Value.SetGrouping(_compendiumGrouping, tab.Key);
                                    }
                                    MediatorService.Publish(_table.Value.Draw(_windowState, new Vector2(0, 0)));

                                }
                            }
                        }
                    }

                }
            }
        }
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
                                this.MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWindow)));
                            }

                            if (ImGui.MenuItem("更新日志"))
                            {
                                this.MediatorService.Publish(new OpenGenericWindowMessage(typeof(ChangelogWindow)));
                            }

                            if (ImGui.MenuItem("帮助"))
                            {
                                this.MediatorService.Publish(new OpenGenericWindowMessage(typeof(HelpWindow)));
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
                                this.MediatorService.Publish(new OpenGenericWindowMessage(typeof(SupportDumpWindow)));
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


                    var groupings = _compendiumType.GetGroupings();
                    if (groupings != null)
                    {
                        using (var groupingMenu = ImRaii.Menu("分组方式"))
                        {
                            if (groupingMenu)
                            {
                                if (ImGui.MenuItem("无", _compendiumGrouping == null))
                                {
                                    _compendiumGrouping = null;
                                    _table.Value.ClearGrouping();
                                }
                                foreach (var grouping in groupings)
                                {
                                    if (ImGui.MenuItem(grouping.Name, _compendiumGrouping?.Equals(grouping) ?? false))
                                    {
                                        _compendiumGrouping = grouping;
                                        _compendiumTabs = null;
                                        SetSavedGrouping(grouping.Key);
                                    }
                                }
                            }
                        }
                    }

                    using (var menu = ImRaii.Menu("窗口"))
                    {
                        if (menu)
                        {
                            foreach (var window in _menuWindows)
                            {
                                if (ImGui.MenuItem(window.GenericName))
                                {
                                    this.MediatorService.Publish(new OpenGenericWindowMessage(window.GetType()));
                                }
                            }
                        }
                    }

                    using (var menu = ImRaii.Menu("图鉴"))
                    {
                        if (menu)
                        {
                            foreach (var compendiumType in _compendiumTypes)
                            {
                                if (ImGui.MenuItem(compendiumType.Plural))
                                {
                                    this.MediatorService.Publish(new ToggleCompendiumListMessage(compendiumType));
                                }
                            }
                        }
                    }

                    if (ImGui.IsItemHovered())
                    {
                        using (ImRaii.Tooltip())
                        {
                            ImGui.Text("图鉴是开发中的功能，敬请期待更多内容！");
                        }
                    }

                }
            }
        }

    public override void Invalidate()
    {

    }

    public override FilterConfiguration? SelectedConfiguration => null;
    public override string GenericKey => CompendiumType.Plural.ToLower();
    public override string GenericName => CompendiumType.Plural + " 窗口";
    public override bool DestroyOnClose => true;
    public override bool SaveState => false;
    public override Vector2? DefaultSize => new Vector2(800, 500);
    public override Vector2? MaxSize => null;
    public override Vector2? MinSize => null;

    public ICompendiumType CompendiumType => _compendiumType;

    public override void Initialize()
    {
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _sectionStateTask?.Dispose();
        }
    }
}