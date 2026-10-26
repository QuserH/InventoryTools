using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using Autofac;
using CriticalCommonLib;
using CriticalCommonLib.Addons;
using CriticalCommonLib.Extensions;
using CriticalCommonLib.MarketBoard;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using CriticalCommonLib.Services.Ui;
using DalaMock.Host.Mediator;
using DalaMock.Shared.Interfaces;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic;
using InventoryTools.Logic.Settings;
using DalaMock.Shared.Interfaces;
using Dalamud.Interface;
using InventoryTools.Ui.Widgets;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using InventoryTools.Compendium;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Windows;
using InventoryTools.Lists;
using InventoryTools.Logic.Filters;
using InventoryTools.Mediator;
using InventoryTools.Services;
using InventoryTools.Services.Interfaces;
using InventoryTools.Extensions;
using InventoryTools.Logic.Features;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using ImGuiUtil = OtterGui.ImGuiUtil;
using InventoryItem = FFXIVClientStructs.FFXIV.Client.Game.InventoryItem;
using InventoryTools.Localization;

namespace InventoryTools.Ui
{
    public class FiltersWindow : GenericWindow
    {
        private readonly IListService _listService;
        private readonly IFilterService _filterService;
        private readonly TableService _tableService;
        private readonly IChatUtilities _chatUtilities;
        private readonly ICharacterMonitor _characterMonitor;
        private readonly IUniversalis _universalis;
        private readonly IFileDialogManager _fileDialogManager;
        private readonly IGameUiManager _gameUiManager;
        private readonly InventoryHistory _inventoryHistory;
        private readonly ListImportExportService _importExportService;
        private readonly IComponentContext _context;
        private readonly FiltersWindowLayoutSetting _layoutSetting;
        private readonly ItemSheet _itemSheet;
        private readonly FilterConfiguration.Factory _filterConfigFactory;
        private readonly IEnumerable<ISampleFilter> _sampleFilters;

        private readonly IClipboardService _clipboardService;
        private readonly PopupService _popupService;
        private readonly IKeyState _keyState;
        private readonly IFramework _framework;
        private readonly IPluginLog _pluginLog;
        private readonly HighlightWhenFilter _highlightWhenFilter;
        private readonly HighlightWhenSetting _highlightWhenSetting;
        private readonly IEnumerable<ICompendiumType> _compendiumTypes;
        private IEnumerable<IMenuWindow>? _menuWindows;
        private readonly InventoryToolsConfiguration _configuration;
        private readonly IFont _font;

        public FiltersWindow(ILogger<FiltersWindow> logger, MediatorService mediator, ImGuiService imGuiService,
            InventoryToolsConfiguration configuration, IListService listService, IFilterService filterService,
            TableService tableService, IChatUtilities chatUtilities, ICharacterMonitor characterMonitor,
            IUniversalis universalis, IFileDialogManager fileDialogManager, IGameUiManager gameUiManager,
            HostedInventoryHistory inventoryHistory, ListImportExportService importExportService,
            IComponentContext context, FiltersWindowLayoutSetting layoutSetting, ItemSheet itemSheet,
            FilterConfiguration.Factory filterConfigFactory, IEnumerable<ISampleFilter> sampleFilters,
            IClipboardService clipboardService, PopupService popupService, IKeyState keyState, IFramework framework,
            IPluginLog pluginLog, HighlightWhenFilter highlightWhenFilter, HighlightWhenSetting highlightWhenSetting,
            IEnumerable<ICompendiumType> compendiumTypes, IFont font) : base(logger, mediator, imGuiService, configuration, "筛选器窗口")
        {
            _listService = listService;
            _filterService = filterService;
            _tableService = tableService;
            _chatUtilities = chatUtilities;
            _characterMonitor = characterMonitor;
            _universalis = universalis;
            _fileDialogManager = fileDialogManager;
            _gameUiManager = gameUiManager;
            _inventoryHistory = inventoryHistory;
            _importExportService = importExportService;
            _context = context;
            _layoutSetting = layoutSetting;
            _itemSheet = itemSheet;
            _filterConfigFactory = filterConfigFactory;
            _sampleFilters = sampleFilters;
            _clipboardService = clipboardService;
            _popupService = popupService;
            _keyState = keyState;
            _framework = framework;
            _pluginLog = pluginLog;
            _highlightWhenFilter = highlightWhenFilter;
            _highlightWhenSetting = highlightWhenSetting;
            _compendiumTypes = compendiumTypes.Where(c => c.ShowInListing).OrderBy(c => c.Plural);
            _configuration = configuration;
            _font = font;
            this.Flags = ImGuiWindowFlags.MenuBar;
        }

        public override void Initialize()
        {
            Key = "filters";
            WindowName = "物品";
            _settingsMenu = new PopupMenu("configMenu", PopupMenu.PopupMenuButtons.All,
                new List<PopupMenu.IPopupMenuItem>()
                {
                    new PopupMenu.PopupMenuItemSelectable("怪物窗口", "mobs", OpenMobsWindow,
                        "打开怪物窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("NPC 窗口", "npcs", OpenNpcsWindow,
                        "打开 NPC 窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("副本窗口", "duties", OpenDutiesWindow,
                        "打开副本窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("飞空艇窗口", "airships", OpenAirshipsWindow,
                        "打开飞空艇窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("潜水艇窗口", "submarines", OpenSubmarinesWindow,
                        "打开潜水艇窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("雇员探险窗口", "ventures",
                        OpenRetainerVenturesWindow, "打开雇员探险窗口。"),
                    new PopupMenu.PopupMenuItemSeparator(),
                    new PopupMenu.PopupMenuItemSelectable("帮助", "help", OpenHelpWindow, "打开帮助窗口。"),
                });

            _tabLayout = Utils.GenerateRandomId();
            _addFilterMenu = new PopupMenu("addFilter", PopupMenu.PopupMenuButtons.LeftRight,
                new List<PopupMenu.IPopupMenuItem>()
                {
                    new PopupMenu.PopupMenuItemSelectableAskName("搜索列表", "adf1", "新建搜索列表",
                        AddSearchFilter,
                        "这将创建一个新列表，让你在角色和雇员背包中搜索特定物品。"),
                    new PopupMenu.PopupMenuItemSelectableAskName("排序列表", "af2", "新建排序列表", AddSortFilter,
                        "这将创建一个新列表，让你在角色和雇员背包中搜索特定物品，并确定它们应移动到何处。"),
                    new PopupMenu.PopupMenuItemSelectableAskName("游戏物品列表", "af3", "新建游戏物品列表",
                        AddGameItemFilter, "这将创建一个可搜索游戏内所有物品的列表。"),
                    new PopupMenu.PopupMenuItemSelectableAskName("历史记录列表", "af4", "新建历史列表",
                        AddHistoryFilter,
                        "这将创建一个可查看背包历史变动数据的列表。"),
                    new PopupMenu.PopupMenuItemSelectableAskName("精选列表", "af5", "新建精选列表",
                        AddCuratedFilter, "这将创建一个可手动添加单个物品的列表。"),
                });
            _menuWindows = _context.Resolve<IEnumerable<IMenuWindow>>().OrderBy(c => c.GenericName).Where(c => c.GetType() != this.GetType());
            MediatorService.Subscribe<ListInvalidatedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRepositionedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListAddedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<ListRemovedMessage>(this, _ => Invalidate());
            MediatorService.Subscribe<TeamCraftDataImported>(this, ImportTeamcraftData);
            MediatorService.Subscribe<FocusListMessage>(this, FocusList);
        }

        private void FocusList(FocusListMessage obj)
        {
            if (obj.windowType == this.GetType())
            {
                this.FocusFilter(obj.FilterConfiguration);
            }
        }

        public override bool SaveState => true;

        private string _activeFilter = "";
        private string _tabLayout = "";
        private int _selectedFilterTab;
        private bool _settingsActive;
        public override Vector2? MaxSize { get; } = new(2000, 2000);
        public override Vector2? MinSize { get; } = new(200, 200);
        public override Vector2? DefaultSize { get; } = new(600, 600);
        public override string GenericKey => "filters";
        public override string GenericName => "筛选器";
        public override bool DestroyOnClose => false;

        private bool _addItemBarOpen;

        public bool ShowAddItemBar =>
            SelectedConfiguration is { FilterType: FilterType.CuratedList } &&
            _addItemBarOpen;


        private List<FilterConfiguration>? _filters;
        private PopupMenu _addFilterMenu = null!;

        private PopupMenu _settingsMenu;

        private void PasteListContents(string obj)
        {
            if (SelectedConfiguration != null)
            {
                var importedList = _importExportService.FromTCString(_clipboardService.PasteFromClipboard());
                if (importedList == null)
                {
                    _chatUtilities.PrintError("无法解析剪贴板内容。");
                }
                else
                {
                    _chatUtilities.Print("剪贴板内容已导入。");
                    this.SelectedConfiguration.AddItemsToList(importedList);
                }
            }
        }


        private void CopyListContents(string obj)
        {
            if (SelectedConfiguration != null)
            {
                var tcString = _importExportService.ToTCString(SelectedConfiguration.CuratedItems?.ToList() ?? []);
                _clipboardService.CopyToClipboard(tcString);
                _chatUtilities.Print("精选列表的内容已复制到剪贴板。");
            }
        }

        private void ClearListContents(string arg1, bool arg2)
        {
            if (arg2)
            {
                if (this.SelectedConfiguration != null &&
                    this.SelectedConfiguration.FilterType == FilterType.CuratedList)
                {
                    this.SelectedConfiguration.CuratedItems = new List<CuratedItem>();
                    this.SelectedConfiguration.NeedsRefresh = true;
                }
            }
        }

        private void ImportTeamcraftData(TeamCraftDataImported data)
        {
            if (SelectedConfiguration != null)
            {
                foreach (var item in data.listData)
                {
                    bool isHq = item.Item1 > 1000000;
                    var itemId = item.Item1 % 500000;
                    SelectedConfiguration.AddCuratedItem(new CuratedItem(itemId, item.Item2,
                        isHq ? InventoryItem.ItemFlags.HighQuality : InventoryItem.ItemFlags.None));
                }

                SelectedConfiguration.NeedsRefresh = true;
            }
        }

        private void OpenHelpWindow(string obj)
        {
            MediatorService.Publish(new Mediator.OpenGenericWindowMessage(typeof(HelpWindow)));
        }

        private void OpenDutiesWindow(string obj)
        {
            MediatorService.Publish(new Mediator.OpenGenericWindowMessage(typeof(DutiesWindow)));
        }

        private void OpenAirshipsWindow(string obj)
        {
            MediatorService.Publish(new Mediator.OpenGenericWindowMessage(typeof(AirshipsWindow)));
        }

        private void OpenSubmarinesWindow(string obj)
        {
            MediatorService.Publish(new Mediator.OpenGenericWindowMessage(typeof(SubmarinesWindow)));
        }

        private void OpenRetainerVenturesWindow(string obj)
        {
            MediatorService.Publish(new Mediator.OpenGenericWindowMessage(typeof(RetainerTasksWindow)));
        }

        private void OpenMobsWindow(string obj)
        {
            MediatorService.Publish(new Mediator.OpenGenericWindowMessage(typeof(BNpcsWindow)));
        }

        private void OpenNpcsWindow(string obj)
        {
            MediatorService.Publish(new Mediator.OpenGenericWindowMessage(typeof(ENpcsWindow)));
        }

        private Dictionary<FilterConfiguration, PopupMenu> _popupMenus = new();

        public PopupMenu GetFilterMenu(FilterConfiguration configuration, WindowLayout layout)
        {
            if (!_popupMenus.ContainsKey(configuration))
            {
                _popupMenus[configuration] = new PopupMenu("fm" + configuration.Key, PopupMenu.PopupMenuButtons.Right,
                    new List<PopupMenu.IPopupMenuItem>()
                    {
                        new PopupMenu.PopupMenuItemSelectable("编辑", "ef_" + configuration.Key, EditFilter,
                            "编辑筛选器。"),
                        new PopupMenu.PopupMenuItemSelectableAskName("复制", "df_" + configuration.Key,
                            configuration.Name, DuplicateFilter, "复制筛选器。"),
                        new PopupMenu.PopupMenuItemSelectable(layout == WindowLayout.Tabs ? "左移" : "上移",
                            "mu_" + configuration.Key, MoveFilterUp,
                            layout == WindowLayout.Tabs ? "左移筛选器。" : "上移筛选器。"),
                        new PopupMenu.PopupMenuItemSelectable(layout == WindowLayout.Tabs ? "右移" : "下移",
                            "md_" + configuration.Key, MoveFilterDown,
                            layout == WindowLayout.Tabs ? "右移筛选器。" : "下移筛选器。"),
                        new PopupMenu.PopupMenuItemSelectableConfirm("移除", "rf_" + configuration.Key,
                            "确定要移除该筛选器吗？", RemoveFilter, "移除筛选器。"),
                    }
                );
            }

            return _popupMenus[configuration];
        }

        private void EditFilter(string id)
        {
            id = id.Replace("ef_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                FocusFilter(existingFilter);
                this._settingsActive = true;
            }
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
                }
            }
        }

        private void MoveFilterDown(string id)
        {
            id = id.Replace("md_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                var currentFilter = this.SelectedConfiguration;
                _listService.MoveListDown(existingFilter);
                if (currentFilter != null)
                {
                    FocusFilter(currentFilter);
                }
            }
        }

        private void MoveFilterUp(string id)
        {
            id = id.Replace("mu_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                var currentFilter = this.SelectedConfiguration;
                _listService.MoveListUp(existingFilter);
                if (currentFilter != null)
                {
                    FocusFilter(currentFilter);
                }
            }
        }

        private void DuplicateFilter(string filterName, string id)
        {
            id = id.Replace("df_", "");
            var existingFilter = _listService.GetListByKey(id);
            if (existingFilter != null)
            {
                var newFilter = _listService.DuplicateList(existingFilter, filterName);
                MediatorService.Publish(new ConfigurationWindowEditFilter(newFilter));
                MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWindow)));
                FocusFilter(newFilter);
            }
        }

        public void FocusFilter(FilterConfiguration filterConfiguration, bool showSettings = false)
        {
            var filterConfigurations = Filters;
            if (filterConfigurations.Contains(filterConfiguration))
            {
                _selectedFilterTab = filterConfigurations.IndexOf(filterConfiguration);
                var filterIndex = Filters.Contains(filterConfiguration) ? Filters.IndexOf(filterConfiguration) : -1;
                if (filterIndex != -1)
                {
                    _newTab = filterIndex;
                }

                _applyNewTabTime = DateTime.Now + TimeSpan.FromMilliseconds(10);
                if (showSettings)
                {
                    _settingsActive = true;
                }
            }
        }

        private void AddSearchFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.SearchFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            Invalidate();
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWindow)));
            MediatorService.Publish(new ConfigurationWindowEditFilter(filterConfiguration));
            FocusFilter(filterConfiguration);
        }

        private void AddHistoryFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.HistoryFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            Invalidate();
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWindow)));
            MediatorService.Publish(new ConfigurationWindowEditFilter(filterConfiguration));
            FocusFilter(filterConfiguration);
        }

        private void AddCuratedFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.CuratedList;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            Invalidate();
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWindow)));
            MediatorService.Publish(new ConfigurationWindowEditFilter(filterConfiguration));
            FocusFilter(filterConfiguration);
        }

        private void AddGameItemFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.GameItemFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            Invalidate();
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWindow)));
            MediatorService.Publish(new ConfigurationWindowEditFilter(filterConfiguration));
            FocusFilter(filterConfiguration);
        }

        private void AddSortFilter(string newName, string id)
        {
            var filterConfiguration = _filterConfigFactory.Invoke();
            filterConfiguration.Name = newName;
            filterConfiguration.FilterType = FilterType.SortingFilter;
            _listService.AddDefaultColumns(filterConfiguration);
            _listService.AddList(filterConfiguration);
            Invalidate();
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWindow)));
            MediatorService.Publish(new ConfigurationWindowEditFilter(filterConfiguration));
            FocusFilter(filterConfiguration);
        }

        private List<FilterConfiguration> Filters
        {
            get
            {
                if (_filters == null)
                {
                    _filters = _listService.Lists.Where(c => c.FilterType != FilterType.CraftFilter).ToList();
                }

                return _filters;
            }
        }

        private int? _newTab;
        private DateTime? _applyNewTabTime;

        private bool SwitchNewTab =>
            _newTab != null && _applyNewTabTime != null && _applyNewTabTime.Value <= DateTime.Now;

        public override void OnClose()
        {
            if (SelectedConfiguration != null)
            {
                SelectedConfiguration.Active = false;
            }

            foreach (var filter in Filters)
            {
                if (SelectedConfiguration == filter)
                {
                    filter.Active = false;
                }
            }

            base.OnClose();
        }

        public override unsafe void DrawWindow()
        {
            foreach (var filter in Filters)
            {
                if (SelectedConfiguration == filter)
                {
                    if (filter.Active != true)
                    {
                        filter.NeedsRefresh = true;
                        filter.Active = true;
                    }
                }
                else
                {
                    filter.Active = false;
                }
            }

            DrawMenuBar();
            _popupService.Draw(GetType());
            if (_configuration.FiltersLayout == WindowLayout.Sidebar)
            {
                DrawSidebar();
                DrawMainWindow();
            }
            else if(_configuration.FiltersLayout == WindowLayout.Tabs)
            {
                DrawTabBar();
            }
            else
            {
                DrawMainWindow();
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

                    using (var menu = ImRaii.Menu("编辑"))
                    {
                        if (menu)
                        {
                            if (this.SelectedConfiguration != null)
                            {
                                if (ImGui.MenuItem("清除搜索"))
                                {
                                    _tableService.GetListTable(SelectedConfiguration).ClearFilters();
                                }

                                ImGui.Separator();

                                using (var copyListContentsMenu = ImRaii.Menu("复制列表内容"))
                                {
                                    if (copyListContentsMenu)
                                    {
                                        if (ImGui.MenuItem("Teamcraft 格式"))
                                        {
                                            var searchResults = _tableService.GetListTable(SelectedConfiguration)
                                                .SearchResults;
                                            var tcString = _importExportService.ToTCString(searchResults);
                                            _clipboardService.CopyToClipboard(tcString);
                                            _chatUtilities.Print("列表的内容已复制到剪贴板。");
                                        }

                                        if (ImGui.MenuItem("JSON 格式"))
                                        {
                                            var itemTable = _tableService.GetListTable(SelectedConfiguration);
                                            _clipboardService.CopyToClipboard(itemTable.ExportToJson());
                                        }
                                    }
                                }

                                if (SelectedConfiguration.FilterType == FilterType.CuratedList &&
                                    ImGui.MenuItem("粘贴列表内容"))
                                {
                                    var importedList =
                                        _importExportService.FromTCString(_clipboardService.PasteFromClipboard(),
                                            false);
                                    if (importedList == null)
                                    {
                                        _chatUtilities.PrintError(
                                            "无法解析剪贴板内容。");
                                    }
                                    else
                                    {
                                        _chatUtilities.Print("剪贴板内容已导入。");
                                        SelectedConfiguration.AddItemsToList(importedList);
                                    }
                                }

                                if (SelectedConfiguration.FilterType == FilterType.CuratedList &&
                                    ImGui.MenuItem("清空列表"))
                                {
                                    _popupService.AddPopup(new ConfirmPopup(GetType(), "craftListDelete",
                                        "确定要清除此精选列表吗？",
                                        result =>
                                        {
                                            if (result)
                                            {
                                                SelectedConfiguration.ClearCuratedItems();
                                            }
                                        }));
                                }

                                ImGui.Separator();
                                using (var addCraftListMenu = ImRaii.Menu("添加到制作列表"))
                                {
                                    if (addCraftListMenu)
                                    {
                                        var craftLists = _listService.Lists
                                            .Where(c => c.FilterType == FilterType.CraftFilter &&
                                                        c.CraftListDefault == false)
                                            .OrderBy(c => c.Order)
                                            .ToList();
                                        foreach (var craft in craftLists)
                                        {
                                            if (ImGui.MenuItem(craft.Name))
                                            {
                                                var searchResults = _tableService.GetListTable(SelectedConfiguration)
                                                    .SearchResults;
                                                foreach (var searchResult in searchResults)
                                                {
                                                    craft.CraftList.AddCraftItem(searchResult.ItemId,
                                                        searchResult.Quantity,
                                                        searchResult.Flags);
                                                }

                                                MediatorService.Publish(
                                                    new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                                MediatorService.Publish(new FocusListMessage(typeof(CraftsWindow),
                                                    craft));
                                            }
                                        }

                                        if (craftLists.Count != 0)
                                        {
                                            ImGui.Separator();
                                        }

                                        if (ImGui.MenuItem("新建制作列表"))
                                        {
                                            _popupService.AddPopup(new NamePopup(typeof(FiltersWindow), "newCraftList",
                                                "新建制作列表",
                                                result =>
                                                {
                                                    if (result.Item1)
                                                    {
                                                        var craftList = _listService.AddNewCraftList(result.Item2);
                                                        var searchResults = _tableService
                                                            .GetListTable(SelectedConfiguration)
                                                            .SearchResults;
                                                        foreach (var searchResult in searchResults)
                                                        {
                                                            craftList.CraftList.AddCraftItem(searchResult.ItemId,
                                                                searchResult.Quantity,
                                                                searchResult.Flags);
                                                        }

                                                        MediatorService.Publish(
                                                            new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                                        this.MediatorService.Publish(new FocusListMessage(
                                                            typeof(CraftsWindow),
                                                            craftList));
                                                    }
                                                }));
                                        }

                                        if (ImGui.MenuItem("新建制作列表（临时）"))
                                        {
                                            _popupService.AddPopup(new NamePopup(typeof(FiltersWindow), "newCraftList",
                                                "新建制作列表",
                                                result =>
                                                {
                                                    if (result.Item1)
                                                    {
                                                        var craftList =
                                                            _listService.AddNewCraftList(result.Item2, true);
                                                        var searchResults = _tableService
                                                            .GetListTable(SelectedConfiguration)
                                                            .SearchResults;
                                                        foreach (var searchResult in searchResults)
                                                        {
                                                            craftList.CraftList.AddCraftItem(searchResult.ItemId,
                                                                searchResult.Quantity,
                                                                searchResult.Flags);
                                                        }

                                                        MediatorService.Publish(
                                                            new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                                        this.MediatorService.Publish(new FocusListMessage(
                                                            typeof(CraftsWindow),
                                                            craftList));
                                                    }
                                                }));
                                        }
                                    }
                                }

                                using (var curatedListMenu = ImRaii.Menu("添加到精选列表"))
                                {
                                    if (curatedListMenu)
                                    {
                                        var curatedLists = _listService.Lists
                                            .Where(c => c.FilterType == FilterType.CuratedList)
                                            .OrderBy(c => c.Order)
                                            .ToList();
                                        foreach (var curatedList in curatedLists)
                                        {
                                            if (ImGui.MenuItem(curatedList.Name))
                                            {
                                                var searchResults = _tableService.GetListTable(SelectedConfiguration)
                                                    .SearchResults;
                                                foreach (var searchResult in searchResults)
                                                {
                                                    curatedList.AddCuratedItem(new CuratedItem(searchResult.ItemId,
                                                        searchResult.Quantity,
                                                        searchResult.Flags));
                                                }
                                            }
                                        }

                                        if (curatedLists.Count != 0)
                                        {
                                            ImGui.Separator();
                                        }

                                        if (ImGui.MenuItem("新建精选列表"))
                                        {
                                            _popupService.AddPopup(new NamePopup(typeof(FiltersWindow),
                                                "newCuratedList",
                                                "新建精选列表",
                                                result =>
                                                {
                                                    if (result.Item1)
                                                    {
                                                        var curatedList = _listService.AddNewCuratedList(result.Item2);
                                                        var searchResults = _tableService
                                                            .GetListTable(SelectedConfiguration)
                                                            .SearchResults;
                                                        foreach (var searchResult in searchResults)
                                                        {
                                                            curatedList.AddCuratedItem(new CuratedItem(
                                                                searchResult.ItemId,
                                                                searchResult.Quantity,
                                                                searchResult.Flags));
                                                        }

                                                        this.MediatorService.Publish(new FocusListMessage(
                                                            typeof(FiltersWindow),
                                                            curatedList));
                                                        curatedList.NeedsRefresh = true;
                                                    }
                                                }));
                                        }
                                    }
                                }
                            }
                        }
                    }


                    using (var menu = ImRaii.Menu("视图"))
                    {
                        if (menu)
                        {
                            if (ImGui.MenuItem("标签页", "",
                                    _layoutSetting.CurrentValue(_configuration) == WindowLayout.Tabs))
                            {
                                _layoutSetting.UpdateFilterConfiguration(_configuration, WindowLayout.Tabs);
                            }

                            if (ImGui.MenuItem("侧边栏", "",
                                    _layoutSetting.CurrentValue(_configuration) == WindowLayout.Sidebar))
                            {
                                _layoutSetting.UpdateFilterConfiguration(_configuration, WindowLayout.Sidebar);
                            }

                            if (ImGui.MenuItem("单页", "",
                                    _layoutSetting.CurrentValue(_configuration) == WindowLayout.Single))
                            {
                                _layoutSetting.UpdateFilterConfiguration(_configuration, WindowLayout.Single);
                            }
                        }
                    }

                    if (ImGui.MenuItem("导出"))
                    {
                        if (SelectedConfiguration != null)
                        {
                            var itemTable = _tableService.GetListTable(SelectedConfiguration);
                            _fileDialogManager.SaveFileDialog("保存为 CSV", "*.csv", "export.csv", ".csv",
                                (b, s) => { SaveCallback(itemTable, b, s); }, null, true);
                        }
                    }

                    using (var menu = ImRaii.Menu("市场"))
                    {
                        if (menu)
                        {
                            if (ImGui.MenuItem("刷新所有价格"))
                            {
                                var activeCharacter = _characterMonitor.ActiveCharacter;
                                if (activeCharacter != null && SelectedConfiguration != null)
                                {
                                    var itemTable = _tableService.GetListTable(SelectedConfiguration);
                                    foreach (var item in itemTable.RenderSearchResults)
                                    {
                                        _universalis.QueuePriceCheck(item.Item.RowId, activeCharacter.WorldId);
                                    }
                                }
                            }
                        }
                    }

                    using (var menu = ImRaii.Menu("列表"))
                    {
                        if (menu)
                        {
                            using (var addMenu = ImRaii.Menu("添加"))
                            {
                                if (addMenu)
                                {
                                    if (ImGui.MenuItem("搜索列表"))
                                    {
                                        _popupService.AddPopup(new NamePopup(GetType(), "addSearchList", "", result =>
                                        {
                                            if (result.Item1)
                                            {
                                                AddSearchFilter(result.Item2, "");
                                            }
                                        }));
                                    }

                                    if (ImGui.MenuItem("排序列表"))
                                    {
                                        _popupService.AddPopup(new NamePopup(GetType(), "addSortList", "", result =>
                                        {
                                            if (result.Item1)
                                            {
                                                AddSortFilter(result.Item2, "");
                                            }
                                        }));
                                    }

                                    if (ImGui.MenuItem("游戏物品列表"))
                                    {
                                        _popupService.AddPopup(new NamePopup(GetType(), "addGameItemList", "", result =>
                                        {
                                            if (result.Item1)
                                            {
                                                AddGameItemFilter(result.Item2, "");
                                            }
                                        }));
                                    }

                                    if (ImGui.MenuItem("精选列表"))
                                    {
                                        _popupService.AddPopup(new NamePopup(GetType(), "addCuratedList", "", result =>
                                        {
                                            if (result.Item1)
                                            {
                                                AddCuratedFilter(result.Item2, "");
                                            }
                                        }));
                                    }

                                    if (ImGui.MenuItem("历史记录列表"))
                                    {
                                        _popupService.AddPopup(new NamePopup(GetType(), "addHistoryList", "", result =>
                                        {
                                            if (result.Item1)
                                            {
                                                AddHistoryFilter(result.Item2, "");
                                            }
                                        }));
                                    }
                                }
                            }

                            using (var addMenu = ImRaii.Menu("添加（预配置）"))
                            {
                                if (addMenu)
                                {
                                    foreach (var defaultFilter in _sampleFilters.OrderBy(c => c.Name))
                                    {
                                        if (defaultFilter.SampleFilterType == SampleFilterType.Default)
                                        {
                                            if (ImGui.MenuItem(defaultFilter.Name))
                                            {
                                                _popupService.AddPopup(new NamePopup(GetType(), "addDefault" + defaultFilter.Name, defaultFilter.SampleDefaultName, result =>
                                                {
                                                    if (result.Item1)
                                                    {
                                                        var newFilter = defaultFilter.AddFilter();
                                                        newFilter.Name = result.Item2;
                                                        Invalidate();
                                                        MediatorService.Publish(new OpenGenericWindowMessage(typeof(ConfigurationWindow)));
                                                        MediatorService.Publish(new ConfigurationWindowEditFilter(newFilter));
                                                        FocusFilter(newFilter);
                                                    }
                                                }));
                                            }

                                            ImGuiUtil.HoverTooltip(defaultFilter.SampleDescription);
                                        }
                                    }
                                }
                            }

                            using (var addMenu = ImRaii.Menu("导入/导出"))
                            {
                                if (addMenu)
                                {
                                    if (ImGui.MenuItem("导出当前列表（分享码）"))
                                    {
                                        if (SelectedConfiguration != null)
                                        {
                                            var base64 = _importExportService.ToBase64(SelectedConfiguration);
                                            _clipboardService.CopyToClipboard(base64);
                                            _chatUtilities.PrintClipboardMessage("[导出] ", "筛选器配置");
                                        }
                                    }

                                    if (ImGui.MenuItem("导入列表（分享码）"))
                                    {
                                        _popupService.AddPopup(new MultiLineTextPopup(GetType(), "addSearchList", "请在下方输入有效的列表分享码，然后点击确定导入。", result =>
                                        {
                                            if (result.Item1)
                                            {
                                                var importData = result.Item2;
                                                if (importData == "")
                                                {
                                                    _chatUtilities.PrintClipboardMessage("[导入] ", "必须粘贴通过导出功能生成的列表或他人分享的列表，然后点击确定。");
                                                }
                                                else
                                                {
                                                    try
                                                    {
                                                        if (_importExportService.FromBase64(importData,
                                                                out var newList))
                                                        {
                                                            _chatUtilities.PrintClipboardMessage("[导入] ", "列表导入成功。");
                                                            _listService.AddList(newList);
                                                        }
                                                        else
                                                        {
                                                            _chatUtilities.PrintClipboardMessage("[导入] ", "导入字符串中发现无效数据。请确保该字符串有效。");
                                                        }
                                                    }
                                                    catch (ListImportVersionException e)
                                                    {
                                                        _chatUtilities.PrintClipboardMessage("[Import] ",
                                                            string.Format("此列表已不再兼容。列表版本为 {0}，需要版本 {1}。",
                                                                e.ImportingVersion?.ToString() ?? "0", e.RequiredVersion));
                                                    }
                                                }
                                            }
                                        }));
                                    }
                                }
                            }

                            ImGui.NewLine();

                            var windowGroups = _listService.Lists.GroupBy(c => c.FilterType).OrderBySequence(
                            [
                                FilterType.SearchFilter, FilterType.SortingFilter, FilterType.GameItemFilter,
                                FilterType.HistoryFilter, FilterType.CuratedList, FilterType.CraftFilter
                            ], grouping => grouping.Key).ToList();
                            for (var index = 0; index < windowGroups.Count; index++)
                            {
                                var windowGroup = windowGroups[index];
                                ImGui.Text(windowGroup.Key.FormattedName());
                                ImGui.Separator();
                                foreach (var window in windowGroup)
                                {
                                    if (ImGui.MenuItem(window.Name, "", SelectedConfiguration == window))
                                    {
                                        if (window.FilterType == FilterType.CraftFilter)
                                        {
                                            if (_keyState[VirtualKey.CONTROL])
                                            {
                                                this.MediatorService.Publish(
                                                    new OpenStringWindowMessage(typeof(FilterWindow), window.Key));
                                            }
                                            else
                                            {
                                                MediatorService.Publish(
                                                    new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                                MediatorService.Publish(new FocusListMessage(typeof(CraftsWindow),
                                                    window));
                                            }
                                        }
                                        else
                                        {
                                            if (_keyState[VirtualKey.CONTROL])
                                            {
                                                this.MediatorService.Publish(
                                                    new OpenStringWindowMessage(typeof(FilterWindow), window.Key));
                                            }
                                            else
                                            {
                                                FocusFilter(window);
                                            }
                                        }
                                    }

                                    ImGuiUtil.HoverTooltip("[CTRL] 在新窗口中打开。");
                                }

                                if (index != windowGroups.Count - 1)
                                {
                                    ImGui.NewLine();
                                }
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
                                        this.MediatorService.Publish(new OpenGenericWindowMessage(window.GetType()));
                                    }
                                }
                            }
                        }
                    }

                    using (var menu = ImRaii.Menu("图鉴"))
                    {
                        if (menu)
                        {
                            if (ImGui.Selectable("图鉴查看器"))
                            {
                                this.MediatorService.Publish(new OpenGenericWindowMessage(typeof(CompendiumTypesWindow)));
                            }
                            ImGui.Separator();
                            foreach (var compendiumType in _compendiumTypes)
                            {
                                if (compendiumType.ShowInListing && ImGui.MenuItem(compendiumType.Plural))
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

        private void DrawMainWindow()
        {
            using (var mainChild = ImRaii.Child("Main",new Vector2(-1, -1) * ImGui.GetIO().FontGlobalScale, false,ImGuiWindowFlags.HorizontalScrollbar))
            {
                if (mainChild.Success)
                {
                    var isWindowFocused = ImGui.IsWindowFocused();
                    var filterConfigurations = Filters;

                    if (filterConfigurations.Count == 0)
                    {
                        using (var contentChild = ImRaii.Child("Content", new Vector2(0, 0) * ImGui.GetIO().FontGlobalScale, true))
                        {
                            if (contentChild.Success)
                            {
                                ImGui.TextUnformatted(
                                    "点击左下角的 + 按钮添加制作列表开始使用。");
                            }
                        }
                    }

                    for (var index = 0; index < filterConfigurations.Count; index++)
                    {
                        if (_selectedFilterTab == index)
                        {
                            var filterConfiguration = filterConfigurations[index];
                            if (isWindowFocused)
                            {
                                if (_configuration.SwitchFiltersAutomatically &&
                                    _configuration.ActiveUiFilter != filterConfiguration.Key &&
                                    _configuration.ActiveUiFilter != null)
                                {
                                    _framework.RunOnFrameworkThread(() =>
                                    {
                                        _listService.ToggleActiveUiList(filterConfiguration);
                                    });
                                }
                            }

                            var itemTable = _tableService.GetListTable(filterConfiguration);

                            if (_settingsActive)
                            {
                                DrawSettingsPanel(filterConfiguration);
                            }
                            else
                            {
                                var activeFilter = DrawFilter(itemTable, filterConfiguration);
                                if (_activeFilter != activeFilter && ImGui.IsWindowFocused())
                                {
                                    if (_configuration.SwitchFiltersAutomatically &&
                                        _configuration.ActiveUiFilter != filterConfiguration.Key &&
                                        _configuration.ActiveUiFilter != null)
                                    {
                                        _framework.RunOnFrameworkThread(() =>
                                        {
                                            _listService.ToggleActiveUiList(filterConfiguration);
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void DrawAddItemBar()
        {
            if (ShowAddItemBar)
            {
                ImGui.SameLine();
                using (var addItemChild = ImRaii.Child("AddItem", new Vector2(-1, -1) * ImGui.GetIO().FontGlobalScale, true))
                {
                    if (addItemChild.Success)
                    {
                        var filterConfiguration = SelectedConfiguration;
                        if (filterConfiguration is { FilterType: FilterType.CuratedList })
                        {
                            ImGui.TextUnformatted("添加新物品");
                            var searchString = SearchString;
                            ImGui.InputText("##ItemSearch", ref searchString, 50);
                            if (_searchString != searchString)
                            {
                                SearchString = searchString;
                            }

                            ImGui.SameLine();
                            var clearSearchCursorX = ImGui.GetCursorPosX();
                            if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Times, ref clearSearchCursorX))
                            {
                                SearchString = "";
                            }

                            ImGuiUtil.HoverTooltip("清除当前搜索。");

                            ImGui.Separator();
                            if (_searchString == "")
                            {
                                ImGui.TextUnformatted("输入文字进行搜索……");
                            }

                            using var table = ImRaii.Table("", 2, ImGuiTableFlags.None);
                            if (!table || !table.Success)
                                return;

                            ImGui.TableSetupColumn("名称", ImGuiTableColumnFlags.None, 200);
                            ImGui.TableSetupColumn("", ImGuiTableColumnFlags.None, 16);

                            foreach (var datum in SearchItems)
                            {
                                ImGui.TableNextRow();
                                DrawSearchRow(filterConfiguration, datum);
                            }
                        }
                    }
                }
            }
        }

        private void DrawSidebar()
        {
            var filterConfigurations = Filters;
            using (var sideMenuChild = ImRaii.Child("SideMenu", new Vector2(180, -1) * ImGui.GetIO().FontGlobalScale, true))
            {
                if (sideMenuChild.Success)
                {
                    using (var craftListChild = ImRaii.Child("CraftList", new Vector2(0, -28) * ImGui.GetIO().FontGlobalScale, false))
                    {
                        if (craftListChild.Success)
                        {
                            for (var index = 0; index < filterConfigurations.Count; index++)
                            {
                                var filterConfiguration = filterConfigurations[index];
                                if (ImGui.Selectable(filterConfiguration.Name + "###fl" + filterConfiguration.Key,
                                        index == _selectedFilterTab))
                                {
                                    _selectedFilterTab = index;
                                    if (_configuration.SwitchFiltersAutomatically &&
                                        _configuration.ActiveUiFilter != filterConfiguration.Key &&
                                        _configuration.ActiveUiFilter != null)
                                    {
                                        _framework.RunOnFrameworkThread(() =>
                                        {
                                            _listService.ToggleActiveUiList(filterConfiguration);
                                        });
                                    }
                                }

                                GetFilterMenu(filterConfiguration, WindowLayout.Sidebar).Draw();
                            }
                        }
                    }

                    using (var commandBarChild = ImRaii.Child("CommandBar", new Vector2(0, 0) * ImGui.GetIO().FontGlobalScale, false))
                    {
                        if (commandBarChild.Success)
                        {
                            float height = ImGui.GetWindowSize().Y;
                            ImGui.SetCursorPosY(height - 24 * ImGui.GetIO().FontGlobalScale);
                            var addFilterCursorX = ImGui.GetCursorPosX();
                            if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Plus, ref addFilterCursorX))
                            {
                            }

                            _addFilterMenu.Draw();

                            ImGuiUtil.HoverTooltip("添加新列表。");
                        }
                    }
                }
            }

            ImGui.SameLine();
        }

        private unsafe void DrawTabBar()
        {
            using (var tabBar = ImRaii.TabBar("InventoryTabs" + _tabLayout, ImGuiTabBarFlags.FittingPolicyScroll | ImGuiTabBarFlags.ListPopupButton))
            {
                if (!tabBar.Success) return;
                var filterConfigurations = Filters;
                for (var index = 0; index < filterConfigurations.Count; index++)
                {
                    var filterConfiguration = filterConfigurations[index];
                    var itemTable = _tableService.GetListTable(filterConfiguration);

                    if (filterConfiguration.DisplayInTabs)
                    {
                        var imGuiTabItemFlags = _newTab == index && SwitchNewTab ? ImGuiTabItemFlags.SetSelected : ImGuiTabItemFlags.None;
                        using var id = ImRaii.PushId(index);

                        using (var tabItem = ImRaii.TabItem(filterConfiguration.NameFormatted + "##" + filterConfiguration.Key, imGuiTabItemFlags))
                        {
                            GetFilterMenu(filterConfiguration, WindowLayout.Tabs).Draw();

                            if (SwitchNewTab && _newTab != null && _newTab == index)
                            {
                                _newTab = null;
                                _applyNewTabTime = null;
                            }
                            if (!tabItem.Success) continue;

                            _selectedFilterTab = index;
                            if (_settingsActive)
                            {
                                DrawSettingsPanel(filterConfiguration);
                            }
                            else
                            {
                                var activeFilter = DrawFilter(itemTable, filterConfiguration);
                                if (_activeFilter != activeFilter && ImGui.IsWindowFocused())
                                {
                                    if (_configuration.SwitchFiltersAutomatically &&
                                        _configuration.ActiveUiFilter != filterConfiguration.Key &&
                                        _configuration.ActiveUiFilter != null)
                                    {
                                        _framework.RunOnFrameworkThread(() =>
                                        {
                                            _listService.ToggleActiveUiList(filterConfiguration);
                                        });
                                    }
                                }
                            }
                        }
                    }
                }

                if (_configuration.ShowFilterTab)
                {
                    using (var tabItem = ImRaii.TabItem("全部列表"))
                    {
                        if (tabItem)
                        {
                            using (var child = ImRaii.Child("filterLeft",
                                       new Vector2(100, -1) * ImGui.GetIO().FontGlobalScale,
                                       true))
                            {
                                if (child.Success)
                                {
                                    for (var index = 0; index < filterConfigurations.Count; index++)
                                    {
                                        var filterConfiguration = filterConfigurations[index];
                                        if (ImGui.Selectable(
                                                filterConfiguration.Name + "###fl" + filterConfiguration.Key,
                                                index == _selectedFilterTab))
                                        {
                                            if (_configuration.SwitchFiltersAutomatically &&
                                                _configuration.ActiveUiFilter != filterConfiguration.Key)
                                            {
                                                _framework.RunOnFrameworkThread(() =>
                                                {
                                                    _listService.ToggleActiveUiList(filterConfiguration);
                                                });
                                            }

                                            _selectedFilterTab = index;
                                        }
                                    }
                                }
                            }

                            ImGui.SameLine();
                            using (var child = ImRaii.Child("filterRight", new Vector2(-1, -1), true,
                                       ImGuiWindowFlags.HorizontalScrollbar))
                            {
                                if (child.Success)
                                {
                                    for (var index = 0; index < filterConfigurations.Count; index++)
                                    {
                                        if (_selectedFilterTab == index)
                                        {
                                            var filterConfiguration = filterConfigurations[index];
                                            var table = _tableService.GetListTable(filterConfiguration);
                                            var activeFilter = DrawFilter(table, filterConfiguration);
                                            if (_activeFilter != activeFilter)
                                            {
                                                if (_configuration.SwitchFiltersAutomatically &&
                                                    _configuration.ActiveUiFilter != filterConfiguration.Key &&
                                                    _configuration.ActiveUiFilter != null)
                                                {
                                                    _framework.RunOnFrameworkThread(() =>
                                                    {
                                                        _listService.ToggleActiveUiList(
                                                            filterConfiguration);
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (ImGui.TabItemButton("+", ImGuiTabItemFlags.Trailing | ImGuiTabItemFlags.NoTooltip))
                {
                }

                ImGuiUtil.HoverTooltip("添加新列表");

                _addFilterMenu.Draw();
            }
        }
        private string? _newName;
        private string _filterSearch = "";
        private void DrawSettingsPanel(FilterConfiguration filterConfiguration)
        {
            using (var contentChild = ImRaii.Child("Content", new Vector2(0, -44) * ImGui.GetIO().FontGlobalScale, true))
            {
                if (contentChild.Success)
                {
                    var filterName = _newName ?? filterConfiguration.Name;
                    var labelName = "##" + filterConfiguration.Key;
                    if (ImGui.CollapsingHeader("常规",
                            ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.CollapsingHeader))
                    {
                        ImGui.SetNextItemWidth(100);
                        ImGui.LabelText(labelName + "FilterNameLabel", "名称：");
                        ImGui.SameLine();
                        ImGui.InputText(labelName + "FilterName", ref filterName, 100);
                        if (filterName != _newName && filterName != filterConfiguration.Name)
                        {
                            _newName = filterName;
                        }

                        if (_newName != null)
                        {
                            ImGui.SameLine();
                            if (ImGui.Button("保存"))
                            {
                                filterConfiguration.Name = _newName;
                                Invalidate();
                                _newName = null;
                            }
                        }

                        ImGui.NewLine();
                        if (ImGui.Button("导出配置到剪贴板"))
                        {
                            var base64 = _importExportService.ToBase64(filterConfiguration);
                            _clipboardService.CopyToClipboard(base64);
                            _chatUtilities.PrintClipboardMessage("[导出] ", "筛选器配置");
                        }

                        var filterType = filterConfiguration.FormattedFilterType;
                        ImGui.SetNextItemWidth(100);
                        ImGui.LabelText(labelName + "FilterTypeLabel", "列表类型：");
                        ImGui.SameLine();
                        ImGui.TextDisabled(filterType);

                    }

                    var filterSearch = _filterSearch;
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                    if (ImGui.InputTextWithHint("##SearchFilter", "搜索……", ref filterSearch, 100))
                    {
                        _filterSearch = filterSearch;
                    }
                    ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);

                    using (var tabBar = ImRaii.TabBar("ConfigTabs", ImGuiTabBarFlags.FittingPolicyScroll))
                    {
                        if (tabBar.Success)
                        {
                            var groupedFilters = _filterService.GroupedFilters;
                            groupedFilters = groupedFilters.ToDictionary(c => c.Key, c => c.Value.Where(f => f.Name.ToLower().Contains(_filterSearch.ToLower()) || f.HelpText.ToLower().Contains(_filterSearch.ToLower())).ToList());
                            var hasResults = false;
                            foreach (var group in groupedFilters)
                            {
                                if (group.Value.Count == 0)
                                {
                                    continue;
                                }
                                hasResults = true;
                                var hasValuesSet = false;
                                foreach (var filter in group.Value)
                                {
                                    if (filter.HasValueSet(filterConfiguration) && filter.AvailableIn.HasFlag(filterConfiguration.FilterType))
                                    {
                                        hasValuesSet = true;
                                        break;
                                    }
                                }

                                using var color = ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.HealerGreen,
                                    hasValuesSet);

                                var hasValues = group.Value.Any(filter =>
                                    filter.AvailableIn.HasFlag(FilterType.SearchFilter) &&
                                    filterConfiguration.FilterType.HasFlag(
                                        FilterType.SearchFilter)
                                    ||
                                    (filter.AvailableIn.HasFlag(FilterType.SortingFilter) &&
                                     filterConfiguration.FilterType.HasFlag(FilterType
                                         .SortingFilter))
                                    ||
                                    (filter.AvailableIn.HasFlag(FilterType.CraftFilter) &&
                                     filterConfiguration.FilterType.HasFlag(FilterType
                                         .CraftFilter))
                                    ||
                                    (filter.AvailableIn.HasFlag(FilterType.HistoryFilter) &&
                                     filterConfiguration.FilterType.HasFlag(FilterType
                                         .HistoryFilter))
                                    ||
                                    (filter.AvailableIn.HasFlag(FilterType.CuratedList) &&
                                     filterConfiguration.FilterType.HasFlag(FilterType
                                         .CuratedList))
                                    ||
                                    (filter.AvailableIn.HasFlag(FilterType.GameItemFilter) &&
                                     filterConfiguration.FilterType.HasFlag(FilterType
                                         .GameItemFilter)));
                                if (hasValues)
                                {
                                    using (var tabItem = ImRaii.TabItem(group.Key.FormattedName()))
                                    {
                                        if (!tabItem.Success) continue;
                                        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudWhite))
                                        {
                                            if (group.Key is FilterCategory.CraftColumns or FilterCategory.Columns)
                                            {
                                                using (var craftColumns = ImRaii.Child("craftColumns", new (0, -100)))
                                                {
                                                    if (craftColumns.Success)
                                                    {
                                                        var filter = group.Value.SingleOrDefault(c => c is CraftColumnsFilter or ColumnsFilter);
                                                        if (filter != null)
                                                        {
                                                            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                                                            filter.Draw(filterConfiguration);
                                                            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                                                            ImGui.Separator();
                                                        }
                                                    }
                                                }
                                                using (var otherFilters = ImRaii.Child("otherFilters", new (0, 0)))
                                                {
                                                    if (otherFilters.Success)
                                                    {
                                                        foreach (var filter in group.Value.Where(c => c is not CraftColumnsFilter && c is not ColumnsFilter))
                                                        {
                                                            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                                                            filter.Draw(filterConfiguration);
                                                            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                                                            ImGui.Separator();
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                foreach (var filter in group.Value)
                                                {
                                                    if ((filter.AvailableIn.HasFlag(FilterType.SearchFilter) &&
                                                         filterConfiguration.FilterType.HasFlag(FilterType.SearchFilter)
                                                         ||
                                                         (filter.AvailableIn.HasFlag(FilterType.SortingFilter) &&
                                                          filterConfiguration.FilterType.HasFlag(FilterType
                                                              .SortingFilter))
                                                         ||
                                                         (filter.AvailableIn.HasFlag(FilterType.CraftFilter) &&
                                                          filterConfiguration.FilterType
                                                              .HasFlag(FilterType.CraftFilter))
                                                         ||
                                                         (filter.AvailableIn.HasFlag(FilterType.HistoryFilter) &&
                                                          filterConfiguration.FilterType.HasFlag(FilterType
                                                              .HistoryFilter))
                                                         ||
                                                         (filter.AvailableIn.HasFlag(FilterType.CuratedList) &&
                                                          filterConfiguration.FilterType.HasFlag(FilterType
                                                              .CuratedList))
                                                         ||
                                                         (filter.AvailableIn.HasFlag(FilterType.GameItemFilter) &&
                                                          filterConfiguration.FilterType.HasFlag(FilterType
                                                              .GameItemFilter))
                                                        ))
                                                    {
                                                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                                                        filter.Draw(filterConfiguration);
                                                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                                                        ImGui.Separator();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (!hasResults)
                            {
                                using (var tabItem = ImRaii.TabItem("未找到结果"))
                                {
                                    if (tabItem.Success)
                                    {

                                    }
                                }
                            }
                        }
                    }
                }
            }

            using (var bottomBarChild = ImRaii.Child("BottomBar", new Vector2(0, 0), true, ImGuiWindowFlags.NoScrollbar))
            {
                if (bottomBarChild.Success)
                {
                    ImGuiService.VerticalCenter(
                        "你正在编辑列表的配置。点击右侧的勾选以保存配置。");

                    ImGui.SameLine();
                    float width = ImGui.GetWindowSize().X;
                    ImGui.SetCursorPosX(width - 42 * ImGui.GetIO().FontGlobalScale);
                    ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                    var closeSettingsCursorX = ImGui.GetCursorPosX();
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Check, ref closeSettingsCursorX))
                    {
                        _settingsActive = false;
                    }

                    ImGuiUtil.HoverTooltip("返回筛选器。");
                }
            }
        }

        public unsafe string DrawFilter(FilterTable itemTable, FilterConfiguration filterConfiguration)
        {
            using var mainChild = ImRaii.Child("Main", new Vector2(filterConfiguration.FilterType == FilterType.CuratedList && _addItemBarOpen ? -250 : -1, -1) * ImGui.GetIO().FontGlobalScale, false,
                ImGuiWindowFlags.HorizontalScrollbar);
            if (!mainChild) return filterConfiguration.Key;

            using (var topBarChild = ImRaii.Child("TopBar", new Vector2(0, 40) * ImGui.GetIO().FontGlobalScale, true,
                       ImGuiWindowFlags.NoScrollbar))
            {
                if (topBarChild.Success)
                {
                    var highlightItems = itemTable.HighlightItems;
                    ImGuiService.CenterElement(20 * ImGui.GetIO().FontGlobalScale);
                    ImGui.Checkbox("高亮？" + "###" + itemTable.Key + "VisibilityCheckbox",
                        ref highlightItems);
                    if (highlightItems != itemTable.HighlightItems)
                    {
                        _framework.RunOnFrameworkThread(() =>
                        {
                            _listService.ToggleActiveUiList(itemTable.FilterConfiguration);
                        });
                    }

                    var filterHighlightWhen = _highlightWhenFilter.CurrentValue(filterConfiguration);
                    var configHighlightWhen = _highlightWhenSetting.CurrentValue(_configuration);
                    var highlightMode = filterHighlightWhen == HighlightWhen.UseGlobalConfiguration ? configHighlightWhen : filterHighlightWhen;

                    if (highlightMode == HighlightWhen.WhenSearching)
                    {
                        ImGuiUtil.HoverTooltip(
                            "勾选后，在任意列中搜索时，匹配筛选器的物品将高亮。");
                    }
                    else
                    {
                        ImGuiUtil.HoverTooltip(
                            "勾选后，匹配筛选器的物品将高亮。");
                    }


                    ImGui.SameLine();
                    ImGuiService.CenterElement(20 * ImGui.GetIO().FontGlobalScale);
                    var clearCursorX = ImGui.GetCursorPosX();
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Times, ref clearCursorX))
                    {
                        itemTable.ClearFilters();
                    }

                    ImGuiUtil.HoverTooltip("清除当前搜索。");

                    ImGui.SameLine();
                    float width = ImGui.GetWindowSize().X;


                    if (SelectedConfiguration is { FilterType: FilterType.CuratedList })
                    {
                        ImGui.SameLine();
                        width -= 28 * ImGui.GetIO().FontGlobalScale;
                        if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Search, ref width))
                        {
                            _addItemBarOpen = !_addItemBarOpen;
                        }

                        ImGuiUtil.HoverTooltip("切换添加物品侧边栏。");
                    }

                    ImGui.SameLine();
                    width -= 28 * ImGui.GetIO().FontGlobalScale;
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Edit, ref width))
                    {
                        _settingsActive = !_settingsActive;
                    }

                    ImGuiUtil.HoverTooltip("编辑列表的配置。");
                }
            }
            using (var contentChild = ImRaii.Child("Content", new Vector2(0, -40) * ImGui.GetIO().FontGlobalScale, true,
                       ImGuiWindowFlags.NoScrollbar))
            {
                if (contentChild.Success)
                {
                    // Apply the configured merge to every filter type shown in this window so the
                    // user-created lists all get the same row merging. The result is cached on the
                    // table and only rebuilt when the underlying rows, sort state or options change.
                    itemTable.RenderSearchResults = itemTable.GetMergedDisplayResults(
                        _configuration.MergeCraftListSameSource && _configuration.MergeCraftListApplyToItemWindow,
                        _configuration.MergeCraftListNqHq && _configuration.MergeCraftListNqHqApplyToItemWindow,
                        null);

                    if (filterConfiguration.FilterType == FilterType.CraftFilter)
                    {
                        var craftTable = _tableService.GetCraftTable(filterConfiguration);
                        MediatorService.Publish(craftTable.Draw(new Vector2(0, -400)));
                        MediatorService.Publish(itemTable.Draw(new Vector2(0, 0)));
                    }
                    else
                    {
                        MediatorService.Publish(itemTable.Draw(new Vector2(0, 0)));
                    }

                }
            }

            //Need to have these buttons be determined dynamically or moved elsewhere
            using (var bottomBarChild =
                   ImRaii.Child("BottomBar", new Vector2(0, 0), true, ImGuiWindowFlags.NoScrollbar))
            {
                if (bottomBarChild.Success)
                {
                    var marketCursorX = ImGui.GetCursorPosX();
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.SyncAlt, ref marketCursorX))
                    {
                        var activeCharacter = _characterMonitor.ActiveCharacter;
                        if (activeCharacter != null)
                        {
                            foreach (var item in itemTable.RenderSearchResults)
                            {
                                _universalis.QueuePriceCheck(item.Item.RowId, activeCharacter.WorldId);
                            }
                        }
                    }

                    ImGuiUtil.HoverTooltip("刷新市场价格");
                    ImGui.SameLine();

                    if (filterConfiguration.FilterType == FilterType.CraftFilter &&
                        _gameUiManager.IsWindowVisible(
                            CriticalCommonLib.Services.Ui.WindowName.SubmarinePartsMenu))
                    {
                        var subMarinePartsMenu = _gameUiManager.GetWindow("SubmarinePartsMenu");
                        if (subMarinePartsMenu != null)
                        {
                            ImGui.SameLine();
                            if (ImGui.Button("将部队制作工程加入列表"))
                            {
                                var subAddon = (SubmarinePartsMenuAddon*)subMarinePartsMenu;
                                for (byte i = 0; i < 6; i++)
                                {
                                    var itemRequired = subAddon->GetItem(i);
                                    if (itemRequired != null)
                                    {
                                        var amountLeft = itemRequired.Value.QtyRemaining;
                                        if (amountLeft > 0)
                                        {
                                            _framework.RunOnFrameworkThread(() =>
                                            {
                                                filterConfiguration.CraftList.AddCraftItem(itemRequired.Value.ItemId,amountLeft);
                                                filterConfiguration.NeedsRefresh = true;
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }

                    ImGui.SameLine();
                    ImGuiService.VerticalCenter("待处理的市场请求：" + _universalis.QueuedCount);
                    if (filterConfiguration.FilterType == FilterType.CraftFilter)
                    {
                        ImGui.SameLine();
                        ImGui.TextUnformatted("NQ总花费：" + filterConfiguration.CraftList.MinimumNQCost);
                        ImGui.SameLine();
                        ImGui.TextUnformatted("HQ总花费：" + filterConfiguration.CraftList.MinimumHQCost);
                    }

                    if (filterConfiguration.FilterType == FilterType.CraftFilter)
                    {
                        var craftTable = _tableService.GetCraftTable(filterConfiguration);
                        craftTable.DrawFooterItems();
                        itemTable.DrawFooterItems();
                    }
                    else
                    {
                        itemTable.DrawFooterItems();
                    }

                    var width = ImGui.GetWindowSize().X;


                    width -= 30 * ImGui.GetIO().FontGlobalScale;
                    ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                    ImGui.SetCursorPosX(width);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Cog, ref width))
                    {
                        MediatorService.Publish(new ToggleGenericWindowMessage(typeof(ConfigurationWindow)));
                    }

                    ImGuiUtil.HoverTooltip("打开设置窗口。");

                    ImGui.SetCursorPosY(0);
                    width -= 30 * ImGui.GetIO().FontGlobalScale;
                    ImGui.SetCursorPosX(width);
                    ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Hammer, ref width))
                    {
                        MediatorService.Publish(new ToggleGenericWindowMessage(typeof(CraftsWindow)));
                    }

                    ImGuiUtil.HoverTooltip("打开制作窗口。");

                    if (SelectedConfiguration != null && SelectedConfiguration.FilterType == FilterType.HistoryFilter)
                    {
                        ImGui.SetCursorPosY(0);
                        width -= 30 * ImGui.GetIO().FontGlobalScale;
                        ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                        if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Times, ref width))
                        {
                            ImGui.OpenPopup("confirmHistoryDelete");
                        }

                        var result = InventoryTools.Ui.Widgets.ImGuiUtil.ConfirmPopup("confirmHistoryDelete", new Vector2(300, 100),
                            () =>
                            {
                                ImGui.TextWrapped("确定要清除所有已存储的历史记录吗？");
                            });
                        if (result == true)
                        {
                            _inventoryHistory.ClearHistory();
                        }

                        ImGuiUtil.HoverTooltip("清除你的历史记录。");
                    }

                    //TODO: check me
                    ImGui.SameLine();

                    var totalItems =  itemTable.RenderSearchResults.Count + " 件物品";

                    if (SelectedConfiguration != null && SelectedConfiguration.FilterType == FilterType.GameItemFilter)
                    {
                        totalItems =  itemTable.RenderSearchResults.Count + " 件物品";
                    }

                    if (SelectedConfiguration != null && SelectedConfiguration.FilterType == FilterType.HistoryFilter)
                    {
                        if (_configuration.HistoryEnabled)
                        {
                            totalItems = itemTable.RenderSearchResults.Count + " 条历史记录";
                        }
                        else
                        {
                            totalItems = "历史追踪当前已禁用";
                        }
                    }

                    if (this.Configuration.FiltersLayout == WindowLayout.Single)
                    {
                        var currentList = this.SelectedConfiguration?.Name ?? "无列表";
                        currentList += " | ";
                        totalItems = currentList + totalItems;
                    }

                    var calcTextSize = ImGui.CalcTextSize(totalItems);
                    width -= calcTextSize.X + 15;
                    ImGui.SetCursorPosX(width);
                    ImGuiService.VerticalCenter(totalItems);


                }
            }

            mainChild.Dispose();
            if (filterConfiguration.FilterType == FilterType.CuratedList && _addItemBarOpen)
            {
                DrawAddItemBar();
            }

            return filterConfiguration.Key;
        }

        private void DrawSearchRow(FilterConfiguration filterConfiguration, ItemRow item)
        {
            ImGui.TableNextColumn();
            ImGui.TextWrapped( item.NameString);
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled & ImGuiHoveredFlags.AllowWhenOverlapped & ImGuiHoveredFlags.AllowWhenBlockedByPopup & ImGuiHoveredFlags.AllowWhenBlockedByActiveItem & ImGuiHoveredFlags.AnyWindow) && ImGui.IsMouseReleased(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup("RightClick" + item.RowId);
            }

            using (var popup = ImRaii.Popup("RightClick"+ item.RowId))
            {
                if (popup.Success)
                {
                    MediatorService.Publish(ImGuiService.ImGuiMenuService.DrawRightClickPopup(item));
                }
            }
            ImGui.TableNextColumn();
            using (ImRaii.PushId("s_" + item.RowId))
            {
                var addItemCursorX = ImGui.GetCursorPosX();
                if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Plus, ref addItemCursorX))
                {
                    _framework.RunOnFrameworkThread(() =>
                    {
                        filterConfiguration.AddCuratedItem(new CuratedItem(item.RowId));
                        filterConfiguration.NeedsRefresh = true;
                    });
                }

                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }
            }
        }

        public override FilterConfiguration? SelectedConfiguration
        {
            get
            {
                if (_selectedFilterTab >= 0 && _selectedFilterTab < Filters.Count) return Filters[_selectedFilterTab];
                return null;
            }
        }

        private void SaveCallback(FilterTable filterTable, bool arg1, string arg2)
        {
            if (arg1)
            {
                filterTable.ExportToCsv(arg2);
            }
        }

        public override void Invalidate()
        {
            var selectedConfiguration = SelectedConfiguration;
            _filters = null;
            _tabLayout = Utils.GenerateRandomId();
            if (selectedConfiguration != null)
            {
                FocusFilter(selectedConfiguration);
            }
        }

        private string _searchString = "";
        private List<ItemRow>? _searchItems;
        public List<ItemRow> SearchItems
        {
            get
            {
                if (SearchString == "")
                {
                    _searchItems = new List<ItemRow>();
                    return _searchItems;
                }
                if (_searchItems == null)
                {
                    _searchItems = _itemSheet.Where(c => c.NameString.ToLower().PassesFilter(SearchString.ToLower())).Take(100)
                        .Select(c => _itemSheet.GetRow(c.RowId)).ToList();
                }

                return _searchItems;
            }
        }

        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                _searchItems = null;
            }
        }
    }
}
