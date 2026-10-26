using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.Interface.Widgets;
using AllaganLib.Shared.Extensions;
using Autofac;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Ui.MenuItems;
using DalaMock.Shared.Interfaces;
using Dalamud.Interface;
using InventoryTools.Ui.Widgets;
using OtterGui;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using InventoryTools.Extensions;
using InventoryTools.Logic.Features;
using InventoryTools.Mediator;
using InventoryTools.Services;
using InventoryTools.Services.Interfaces;
using InventoryTools.Ui.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using ImGuiUtil = OtterGui.ImGuiUtil;
using InventoryTools.Localization;

namespace InventoryTools.Ui
{
    public class ConfigurationWindow : GenericWindow, IMenuWindow
    {
        private readonly IPluginLog _pluginLog;
        private readonly ConfigurationWizardService _configurationWizardService;
        private readonly IChatUtilities _chatUtilities;
        private readonly PluginLogic _pluginLogic;
        private readonly IListService _listService;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly SettingPage.Factory _settingPageFactory;
        private readonly FilterConfiguration.Factory _filterConfigurationFactory;
        private readonly IEnumerable<ISampleFilter> _sampleFilters;
        private readonly Func<Type, IConfigPage> _configPageFactory;
        private readonly Func<FilterConfiguration, FilterPage> _filterPageFactory;
        private readonly IComponentContext _context;
        private readonly InventoryToolsConfiguration _configuration;
        private readonly VerticalSplitter _verticalSplitter;
        private readonly IFont _font;
        private IEnumerable<IMenuWindow>? _menuWindows;
        private FilterConfiguration? _nextFilter;

        public ConfigurationWindow(ILogger<ConfigurationWindow> logger,
            IPluginLog pluginLog,
            MediatorService mediator,
            ImGuiService imGuiService,
            InventoryToolsConfiguration configuration,
            ConfigurationWizardService configurationWizardService,
            IChatUtilities chatUtilities,
            PluginLogic pluginLogic,
            IListService listService,
            IServiceScopeFactory serviceScopeFactory,
            Func<Type, IConfigPage> configPageFactory,
            Func<FilterConfiguration, FilterPage> filterPageFactory,
            SettingPage.Factory settingPageFactory,
            FilterConfiguration.Factory filterConfigurationFactory,
            IEnumerable<ISampleFilter> sampleFilters,
            IComponentContext context,
            IFont font) : base(logger,
            mediator,
            imGuiService,
            configuration,
            "设置窗口")
        {
            _pluginLog = pluginLog;
            _configurationWizardService = configurationWizardService;
            _chatUtilities = chatUtilities;
            _pluginLogic = pluginLogic;
            _listService = listService;
            _serviceScopeFactory = serviceScopeFactory;
            _settingPageFactory = settingPageFactory;
            _filterConfigurationFactory = filterConfigurationFactory;
            _sampleFilters = sampleFilters;
            _configPageFactory = configPageFactory;
            _filterPageFactory = filterPageFactory;
            _context = context;
            _configuration = configuration;
            _verticalSplitter = new VerticalSplitter(250, new Vector2(200, 400));
            _font = font;
            this.Flags = ImGuiWindowFlags.MenuBar;
        }

        public override void Initialize()
        {
            WindowName = "设置";
            Key = "configuration";
            _configPages = new List<IConfigPage>();
            _configPages.Add(new SeparatorPageItem("Settings"));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.General));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.Lists));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.Highlighting));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.Items));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.Windows));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.AutoSave));
            _configPages.Add(new SeparatorPageItem("Modules", true));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.MarketBoard));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.ToolTips));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.ContextMenu));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.Hotkeys));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.MobSpawnTracker));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.TitleMenuButtons));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.CraftOverlay));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.CraftTracker));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.History));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.Misc));
            _configPages.Add(_settingPageFactory.Invoke(SettingCategory.Troubleshooting, null, true));
            _configPages.Add(new SeparatorPageItem("Data", true));
            _configPages.Add(_configPageFactory.Invoke(typeof(ListsPage)));
            _configPages.Add(_configPageFactory.Invoke(typeof(CharacterRetainerPage)));

            _addFilterMenu = new PopupMenu("addFilter", PopupMenu.PopupMenuButtons.LeftRight,
                new List<PopupMenu.IPopupMenuItem>()
                {
                    new PopupMenu.PopupMenuItemSelectableAskName("搜索列表", "adf1", "新建搜索列表", AddSearchFilter, "这将创建一个新列表，让你在角色和雇员背包中搜索特定物品。"),
                    new PopupMenu.PopupMenuItemSelectableAskName("排序列表", "af2", "新建排序筛选器", AddSortFilter, "这将创建一个新列表，让你在角色和雇员背包中搜索特定物品，并确定它们应移动到何处。"),
                    new PopupMenu.PopupMenuItemSelectableAskName("游戏物品列表", "af3", "新建游戏物品列表", AddGameItemFilter, "这将创建一个可搜索游戏内所有物品的列表。"),
                    new PopupMenu.PopupMenuItemSelectableAskName("历史记录列表", "af4", "新建历史物品列表", AddHistoryFilter, "这将创建一个可查看背包历史变动数据的列表。"),
                });

            _addSampleMenu = new PopupMenu("addSampleFilter", PopupMenu.PopupMenuButtons.LeftRight, []);

            var sampleId = 0;
            foreach (var sampleFilter in _sampleFilters)
            {
                if (sampleFilter.SampleFilterType == SampleFilterType.Default)
                {
                    _addSampleMenu.Items.Add(new PopupMenu.PopupMenuItemSelectableAskName(sampleFilter.Name,
                        $"sf{sampleId}", sampleFilter.SampleDefaultName, (newName, id) =>
                        {
                            var createdFilter = sampleFilter.AddFilter();
                            createdFilter.Name = newName;
                        }, sampleFilter.SampleDescription));
                    sampleId++;
                }
            }

            _addSampleMenu.Items.Add(new PopupMenu.PopupMenuItemSeparator());

            foreach (var sampleFilter in _sampleFilters)
            {
                if (sampleFilter.SampleFilterType == SampleFilterType.Sample)
                {
                    _addSampleMenu.Items.Add(new PopupMenu.PopupMenuItemSelectableAskName(sampleFilter.Name,
                        $"sf{sampleId}", sampleFilter.SampleDefaultName, (newName, id) =>
                        {
                            var createdFilter = sampleFilter.AddFilter();
                            createdFilter.Name = newName;
                        }, sampleFilter.SampleDescription));
                    sampleId++;
                }
            }

            _settingsMenu = new PopupMenu("configMenu", PopupMenu.PopupMenuButtons.All,
                new List<PopupMenu.IPopupMenuItem>()
                {
                    new PopupMenu.PopupMenuItemSelectable("物品窗口", "filters", OpenFiltersWindow,"打开物品窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("制作窗口", "crafts", OpenCraftsWindow,"打开制作窗口。"),
                    new PopupMenu.PopupMenuItemSeparator(),
                    new PopupMenu.PopupMenuItemSelectable("怪物窗口", "mobs", OpenMobsWindow,"打开怪物窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("NPC 窗口", "npcs", OpenNpcsWindow,"打开 NPC 窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("副本窗口", "duties", OpenDutiesWindow,"打开副本窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("飞空艇窗口", "airships", OpenAirshipsWindow,"打开飞空艇窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("潜水艇窗口", "submarines", OpenSubmarinesWindow,"打开潜水艇窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("雇员探险窗口", "ventures", OpenRetainerVenturesWindow,"打开雇员探险窗口。"),
                    new PopupMenu.PopupMenuItemSeparator(),
                    new PopupMenu.PopupMenuItemSelectable("帮助", "help", OpenHelpWindow,"打开帮助窗口。"),
                });

            _wizardMenu = new PopupMenu("wizardMenu", PopupMenu.PopupMenuButtons.All,
                new List<PopupMenu.IPopupMenuItem>()
                {
                    new PopupMenu.PopupMenuItemSelectable("配置新设置", "configureNew", ConfigureNewSettings,"配置新设置。"),
                    new PopupMenu.PopupMenuItemSelectable("配置全部设置", "configureAll", ConfigureAllSettings,"配置全部设置。"),
                });
            _menuWindows = _context.Resolve<IEnumerable<IMenuWindow>>().OrderBy(c => c.GenericName).Where(c => c.GetType() != this.GetType());

            GenerateFilterPages();
            MediatorService.Subscribe<ListInvalidatedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRepositionedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListAddedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRemovedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ConfigurationWindowEditFilter>(this,  message =>
            {
                Invalidate();
                SetActiveFilter(message.filter);
            });
            MediatorService.Subscribe<ListInvalidatedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRepositionedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListAddedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRemovedMessage>(this, _ => Invalidate());
        }

        private void ListInvalidated(ListInvalidatedMessage obj)
        {
            Invalidate();
        }



        private PopupMenu _wizardMenu = null!;

        private void ConfigureAllSettings(string obj)
        {
            _configurationWizardService.ClearFeaturesSeen();
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWizard)));
        }

        private void ConfigureNewSettings(string obj)
        {
            if (_configurationWizardService.HasNewFeatures)
            {
                MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWizard)));
            }
            else
            {
                _chatUtilities.Print("没有可配置的新设置。");
            }
        }

        private PopupMenu _addFilterMenu = null!;
        private PopupMenu _addSampleMenu = null!;
        private PopupMenu _settingsMenu = null!;

        private void OpenCraftsWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(CraftsWindow)));
        }

        private void OpenFiltersWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(FiltersWindow)));
        }

        private void OpenHelpWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(HelpWindow)));
        }

        private void OpenDutiesWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(DutiesWindow)));
        }

        private void OpenAirshipsWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(AirshipsWindow)));
        }

        private void OpenSubmarinesWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(SubmarinesWindow)));
        }

        private void OpenRetainerVenturesWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(RetainerTasksWindow)));
        }

        private void OpenMobsWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(BNpcsWindow)));
        }

        private void OpenNpcsWindow(string obj)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(ENpcsWindow)));
        }

        private Dictionary<FilterConfiguration, PopupMenu> _popupMenus = new();
        public PopupMenu GetFilterMenu(FilterConfiguration configuration)
        {
            if (!_popupMenus.ContainsKey(configuration))
            {
                _popupMenus[configuration] = new PopupMenu("fm" + configuration.Key, PopupMenu.PopupMenuButtons.Right,
                    new List<PopupMenu.IPopupMenuItem>()
                    {
                        new PopupMenu.PopupMenuItemSelectableAskName("复制", "df_" + configuration.Key, configuration.Name, DuplicateFilter, "复制筛选器。"),
                        new PopupMenu.PopupMenuItemSelectable("上移", "mu_" + configuration.Key, MoveFilterUp, "上移筛选器。"),
                        new PopupMenu.PopupMenuItemSelectable("下移", "md_" + configuration.Key, MoveFilterDown, "下移筛选器。"),
                        new PopupMenu.PopupMenuItemSelectableConfirm("移除", "rf_" + configuration.Key, "确定要移除该筛选器吗？", RemoveFilter, "移除筛选器。"),
                    }
                );
            }

            return _popupMenus[configuration];
        }

        private void RemoveFilter(string id, bool confirmed)
        {
            if (confirmed)
            {
                id = id.Replace("rf_", "");
                var existingFilter = _listService.GetListByKey(id);
                if (existingFilter != null)
                {
                    _listService.RemoveList(existingFilter);
                    ConfigSelectedConfigurationPage--;
                }
            }
        }

        private void MoveFilterDown(string id)
        {
            id = id.Replace("md_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                _listService.MoveListDown(existingFilter);
            }
        }

        private void MoveFilterUp(string id)
        {
            id = id.Replace("mu_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                _listService.MoveListUp(existingFilter);
            }
        }

        private void DuplicateFilter(string filterName, string id)
        {
            id = id.Replace("df_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                _listService.DuplicateList(existingFilter, filterName);
                SetNewFilterActive();
            }
        }

        private void AddSearchFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigurationFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.SearchFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            SetNewFilterActive();
        }

        private void AddHistoryFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigurationFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.HistoryFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            SetNewFilterActive();
        }

        private void AddGameItemFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigurationFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.GameItemFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            SetNewFilterActive();
        }

        private void AddSortFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigurationFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.SortingFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            SetNewFilterActive();
        }


        private int ConfigSelectedConfigurationPage
        {
            get => _configuration.SelectedConfigurationPage;
            set => _configuration.SelectedConfigurationPage = value;
        }

        public void SetActiveFilter(FilterConfiguration configuration)
        {
            if (_filterPages.ContainsKey(configuration.Key))
            {
                _nextFilter = configuration;
            }
        }

        public void GenerateFilterPages()
        {
            var filterConfigurations = _listService.Lists.Where(c => c.FilterType != FilterType.CraftFilter);
            var filterPages = new Dictionary<string, IConfigPage>();
            foreach (var filter in filterConfigurations)
            {
                if (!filterPages.ContainsKey(filter.Key))
                {
                    filterPages.Add(filter.Key, _filterPageFactory.Invoke(filter));
                }
            }

            _filterPages = filterPages;
        }

        public override bool SaveState => true;
        public override Vector2? DefaultSize { get; } = new(700, 700);
        public override Vector2? MaxSize { get; } = new(2000, 2000);
        public override Vector2? MinSize { get; } = new(200, 200);
        public override string GenericKey => "configuration";
        public override string GenericName => "设置";
        public override bool DestroyOnClose => true;
        private List<IConfigPage> _configPages = null!;
        public Dictionary<string, IConfigPage> _filterPages = new Dictionary<string,IConfigPage>();


        private void SetNewFilterActive()
        {
            ConfigSelectedConfigurationPage = _configPages.Count + _filterPages.Count - 2;
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
                            if (ImGui.MenuItem("报告问题"))
                            {
                                "https://github.com/Critical-Impact/AllaganMarket".OpenBrowser();
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
                                this.MediatorService.Publish(new OpenGenericWindowMessage(typeof(SupportDumpWindow)));
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

                    using (var menu = ImRaii.Menu("向导"))
                    {
                        if (menu)
                        {
                            var hasNewFeatures = this._configurationWizardService.HasNewFeatures;
                            using var disabled = ImRaii.Disabled(!hasNewFeatures);
                            if (ImGui.MenuItem("配置新功能"))
                            {
                                MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWizard)));
                            }

                            disabled.Dispose();

                            if (ImGui.MenuItem("重新配置全部功能"))
                            {
                                this._configurationWizardService.ClearFeaturesSeen();
                                MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWizard)));
                            }
                        }
                    }

                    using (var menu = ImRaii.Menu("窗口"))
                    {
                        if (menu)
                        {
                            if (_menuWindows != null)
                            {
                                foreach (var window in _menuWindows)
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
        }

        public override void DrawWindow()
        {
            DrawMenuBar();
            _verticalSplitter.Draw(DrawSideBar, DrawMainWindow);
        }

        private void DrawMainWindow()
        {
            IConfigPage? currentConfigPage = null;

            {
                var count = 0;
                for (var index = 0; index < _configPages.Count; index++)
                {
                    var configPage = _configPages[index];
                    if (configPage.IsMenuItem)
                    {
                        continue;
                    }

                    if (ConfigSelectedConfigurationPage == count)
                    {
                        currentConfigPage = configPage;
                    }


                    if (configPage.ChildPages != null)
                    {
                        foreach (var childPage in configPage.ChildPages)
                        {
                            if (ConfigSelectedConfigurationPage == count)
                            {
                                currentConfigPage = childPage;
                            }
                            count++;
                        }
                    }
                    else
                    {
                        count++;
                    }
                }

                foreach (var filter in _filterPages)
                {
                    count++;
                    if (_nextFilter != null)
                    {
                        if (filter.Value is FilterPage filterPage)
                        {
                            if (filterPage.FilterConfiguration == _nextFilter)
                            {
                                currentConfigPage = filterPage;
                                ConfigSelectedConfigurationPage = count;
                                _nextFilter = null;
                            }
                        }
                    }
                    if (ConfigSelectedConfigurationPage == count)
                    {
                        currentConfigPage = filter.Value;
                    }
                }
            }
            if (currentConfigPage != null)
            {
                MediatorService.Publish(currentConfigPage.Draw());
            }
        }

        private void DrawSideBar()
        {
            using (var menuChild = ImRaii.Child("Menu", new Vector2(0, -28) * ImGui.GetIO().FontGlobalScale,
                       false, ImGuiWindowFlags.NoSavedSettings))
            {
                if (menuChild.Success)
                {

                    var count = 0;
                    for (var index = 0; index < _configPages.Count; index++)
                    {
                        var configPage = _configPages[index];
                        if (configPage.IsMenuItem)
                        {
                            MediatorService.Publish(configPage.Draw());
                        }
                        else
                        {
                            var hasChildren = configPage.ChildPages != null;
                            var isSelected = ConfigSelectedConfigurationPage == count;
                            using (var node = ImRaii.TreeNode(configPage.Name, hasChildren ?  ImGuiTreeNodeFlags.None : isSelected ? ImGuiTreeNodeFlags.Leaf | ImGuiTreeNodeFlags.Selected : ImGuiTreeNodeFlags.Leaf))
                            {
                                if (node)
                                {
                                    if (configPage.ChildPages != null)
                                    {
                                        foreach (var childPage in configPage.ChildPages)
                                        {
                                            isSelected = ConfigSelectedConfigurationPage == count;

                                            using (var subNode = ImRaii.TreeNode(childPage.Name,
                                                       isSelected
                                                           ? ImGuiTreeNodeFlags.Selected |
                                                             ImGuiTreeNodeFlags.Bullet
                                                           : ImGuiTreeNodeFlags.Bullet))
                                            {
                                                if (subNode)
                                                {
                                                }
                                            }

                                            if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
                                            {
                                                ConfigSelectedConfigurationPage = count;
                                            }

                                            count++;
                                        }
                                    }
                                }
                                else
                                {
                                    if (configPage.ChildPages != null)
                                    {
                                        foreach (var childPage in configPage.ChildPages)
                                        {
                                            count++;
                                        }
                                    }
                                }
                            }

                            if (!hasChildren)
                            {
                                if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
                                {
                                    ConfigSelectedConfigurationPage = count;
                                }
                                count++;

                            }
                        }
                    }

                    ImGui.NewLine();
                    ImGui.TextUnformatted("物品列表");
                    ImGui.Separator();

                    var filterIndex = count;
                    foreach (var item in _filterPages)
                    {
                        filterIndex++;
                        using (var subNode = ImRaii.TreeNode(item.Value.Name,
                                   ConfigSelectedConfigurationPage == filterIndex
                                       ? ImGuiTreeNodeFlags.Selected |
                                         ImGuiTreeNodeFlags.Leaf
                                       : ImGuiTreeNodeFlags.Leaf))
                        {
                            if (subNode)
                            {
                            }
                        }

                        if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
                        {
                            ConfigSelectedConfigurationPage = filterIndex;
                        }

                        var filter = _listService.GetListByKey(item.Key);
                        if (filter != null)
                        {
                            GetFilterMenu(filter).Draw();
                        }

                    }
                }
            }

            using (var commandBarChild = ImRaii.Child("CommandBar",
                       new Vector2(0, 0) * ImGui.GetIO().FontGlobalScale, false))
            {
                if (commandBarChild.Success)
                {

                    float height = ImGui.GetWindowSize().Y;
                    ImGui.SetCursorPosY(height - 24 * ImGui.GetIO().FontGlobalScale);

                    var cursorX = ImGui.GetCursorPosX();
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Plus, ref cursorX))
                    {

                    }

                    _addFilterMenu.Draw();
                    ImGuiUtil.HoverTooltip("添加新列表");

                    ImGui.SetCursorPosY(height - 24 * ImGui.GetIO().FontGlobalScale);
                    ImGui.SetCursorPosX(26 * ImGui.GetIO().FontGlobalScale);

                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Lightbulb, ref cursorX))
                    {

                    }

                    _addSampleMenu.Draw();
                    ImGuiUtil.HoverTooltip("添加示例筛选器");

                    var width = ImGui.GetCursorPosX();
                    width -= 24 * ImGui.GetIO().FontGlobalScale;

                    ImGui.SetCursorPosY(height - 24 * ImGui.GetIO().FontGlobalScale);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Bars, ref width))
                    {

                    }

                    _settingsMenu.Draw();


                    width -= 26 * ImGui.GetIO().FontGlobalScale;

                    ImGui.SetCursorPosY(height - 24 * ImGui.GetIO().FontGlobalScale);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.WandMagicSparkles, ref width))
                    {
                        _wizardMenu.Open();
                    }
                    _wizardMenu.Draw();


                    ImGuiUtil.HoverTooltip("启动设置向导。");
                }
            }
        }

        public override void Invalidate()
        {
            GenerateFilterPages();
        }

        public override FilterConfiguration? SelectedConfiguration => null;
    }
}
