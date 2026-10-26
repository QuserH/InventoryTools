using System;
using System.Collections.Generic;
using System.Numerics;
using CriticalCommonLib;
using CriticalCommonLib.Addons;
using CriticalCommonLib.MarketBoard;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using CriticalCommonLib.Services.Ui;
using DalaMock.Host.Mediator;
using DalaMock.Shared.Interfaces;
using FFXIVClientStructs.FFXIV.Client.Game;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic;
using DalaMock.Shared.Interfaces;
using Dalamud.Interface;
using InventoryTools.Ui.Widgets;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using InventoryTools.Lists;
using InventoryTools.Mediator;
using InventoryTools.Services;
using InventoryTools.Services.Interfaces;
using Microsoft.Extensions.Logging;
using ImGuiUtil = OtterGui.ImGuiUtil;
using InventoryTools.Localization;

namespace InventoryTools.Ui
{
    public class FilterWindow : StringWindow
    {
        private readonly TableService _tableService;
        private readonly IListService _listService;
        private readonly ICharacterMonitor _characterMonitor;
        private readonly IUniversalis _universalis;
        private readonly IFileDialogManager _fileDialogManager;
        private readonly IGameUiManager _gameUiManager;
        private readonly IFramework _framework;
        private readonly InventoryToolsConfiguration _configuration;
        private readonly IFont _font;

        public FilterWindow(ILogger<FilterWindow> logger, MediatorService mediator, ImGuiService imGuiService, InventoryToolsConfiguration configuration, TableService tableService, IListService listService, ICharacterMonitor characterMonitor, IUniversalis universalis, IFileDialogManager fileDialogManager, IGameUiManager gameUiManager, IFramework framework, IFont font, string name = "Filter Window") : base(logger, mediator, imGuiService, configuration, name)
        {
            _tableService = tableService;
            _listService = listService;
            _characterMonitor = characterMonitor;
            _universalis = universalis;
            _fileDialogManager = fileDialogManager;
            _gameUiManager = gameUiManager;
            _framework = framework;
            _configuration = configuration;
            _font = font;
        }
        public override void Initialize(string filterKey)
        {
            base.Initialize(filterKey);
            _filterKey = filterKey;
            if (SelectedConfiguration != null)
            {
                Key = "filter_" + filterKey;
                WindowName = "亚拉戈工具箱 - " + SelectedConfiguration.Name;
            }
            else
            {
                Key = "filter_invalid";
                WindowName = "无效列表";
            }

            _settingsMenu = new PopupMenu("configMenu", PopupMenu.PopupMenuButtons.All,
                new List<PopupMenu.IPopupMenuItem>()
                {
                    new PopupMenu.PopupMenuItemSelectable("怪物窗口", "mobs", _ => MediatorService.Publish(new OpenGenericWindowMessage(typeof(BNpcsWindow))),
                        "打开怪物窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("NPC 窗口", "npcs", _ => MediatorService.Publish(new OpenGenericWindowMessage(typeof(ENpcsWindow))),
                        "打开 NPC 窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("副本窗口", "duties", _ => MediatorService.Publish(new OpenGenericWindowMessage(typeof(DutiesWindow))),
                        "打开副本窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("飞空艇窗口", "airships", _ => MediatorService.Publish(new OpenGenericWindowMessage(typeof(AirshipsWindow))),
                        "打开飞空艇窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("潜水艇窗口", "submarines", _ => MediatorService.Publish(new OpenGenericWindowMessage(typeof(SubmarinesWindow))),
                        "打开潜水艇窗口。"),
                    new PopupMenu.PopupMenuItemSelectable("雇员探险窗口", "ventures",_ => MediatorService.Publish(new OpenGenericWindowMessage(typeof(RetainerTasksWindow))),
                        "打开雇员探险窗口。"),
                    new PopupMenu.PopupMenuItemSeparator(),
                    new PopupMenu.PopupMenuItemSelectable("帮助", "help", _ => MediatorService.Publish(new OpenGenericWindowMessage(typeof(HelpWindow))), "打开帮助窗口。"),
                });
        }



        private PopupMenu _settingsMenu;

        public override void Invalidate()
        {

        }

        private string _filterKey;

        public override FilterConfiguration? SelectedConfiguration =>
            _listService.GetListByKey(_filterKey);

        public override string GenericKey { get; } = "filter";
        public override string GenericName { get; } = "筛选器";
        public override bool DestroyOnClose => true;
        public override bool SaveState => true;

        public override void OnClose()
        {
            if (SelectedConfiguration != null)
            {
                SelectedConfiguration.Active = false;
            }
            base.OnClose();
        }

        public override void DrawWindow()
        {
            var filterConfiguration = SelectedConfiguration;
            if (filterConfiguration != null)
            {
                if (filterConfiguration.Active != true)
                {
                    filterConfiguration.Active = true;
                    filterConfiguration.NeedsRefresh = true;
                }

                if (ImGui.IsWindowFocused())
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

                    if (_configuration.SwitchCraftListsAutomatically &&
                        _configuration.ActiveCraftList != filterConfiguration.Key &&
                        _configuration.ActiveCraftList != null &&
                        filterConfiguration.FilterType == FilterType.CraftFilter)
                    {
                        _framework.RunOnFrameworkThread(() =>
                        {
                            _listService.ToggleActiveCraftList(filterConfiguration);
                        });
                    }
                }

                var table = _tableService.GetListTable(filterConfiguration);
                DrawFilter(table, filterConfiguration);
                if (ImGui.IsWindowFocused())
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
                    if (_configuration.SwitchCraftListsAutomatically &&
                        _configuration.ActiveCraftList != filterConfiguration.Key &&
                        _configuration.ActiveCraftList != null && filterConfiguration.FilterType == FilterType.CraftFilter)
                    {
                        _framework.RunOnFrameworkThread(() =>
                        {
                            _listService.ToggleActiveCraftList(filterConfiguration);
                        });
                    }
                }
            }
        }

        public unsafe string DrawFilter(FilterTable itemTable, FilterConfiguration filterConfiguration)
        {
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

                    ImGui.SameLine();
                    ImGuiService.CenterElement(20 * ImGui.GetIO().FontGlobalScale);
                    var clearCursorX = ImGui.GetCursorPosX();
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Times, ref clearCursorX))
                    {
                        itemTable.ClearFilters();
                    }

                    ImGuiUtil.HoverTooltip("清除当前搜索。");

                }
            }
            using (var contentChild = ImRaii.Child("Content", new Vector2(0, -40) * ImGui.GetIO().FontGlobalScale, true,
                       ImGuiWindowFlags.NoScrollbar))
            {
                if (contentChild.Success)
                {
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
                    ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                    var marketCursorX = ImGui.GetCursorPosX();
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.SyncAlt, ref marketCursorX))
                    {
                        var activeCharacter = _characterMonitor.ActiveCharacter;
                        if (activeCharacter != null)
                        {
                            foreach (var item in itemTable.SearchResults)
                            {
                                _universalis.QueuePriceCheck(item.Item.RowId, activeCharacter.WorldId);
                            }
                        }
                    }

                    ImGuiUtil.HoverTooltip("刷新市场价格");
                    ImGui.SameLine();
                    ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                    var csvCursorX = ImGui.GetCursorPosX();
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.FileExport, ref csvCursorX))
                    {
                        _fileDialogManager.SaveFileDialog("保存为 CSV", "*.csv", "export.csv", ".csv",
                            (b, s) => { SaveCallback(itemTable, b, s); }, null, true);
                    }

                    ImGuiUtil.HoverTooltip("导出为 CSV");
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
                                                filterConfiguration.CraftList.AddCraftItem(itemRequired.Value.ItemId, amountLeft);
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
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Bars, ref width))
                    {
                    }
                    _settingsMenu.Draw();

                    width -= 30 * ImGui.GetIO().FontGlobalScale;
                    ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Cog, ref width))
                    {
                        MediatorService.Publish(new ToggleGenericWindowMessage(typeof(ConfigurationWindow)));
                    }

                    ImGuiUtil.HoverTooltip("打开设置窗口。");

                    ImGui.SetCursorPosY(0);
                    width -= 30 * ImGui.GetIO().FontGlobalScale;
                    ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Filter, ref width))
                    {
                        MediatorService.Publish(new ToggleGenericWindowMessage(typeof(FiltersWindow)));
                    }

                    ImGuiUtil.HoverTooltip("打开物品窗口。");

                    ImGui.SetCursorPosY(0);
                    width -= 30 * ImGui.GetIO().FontGlobalScale;
                    ImGuiService.CenterElement(24 * ImGui.GetIO().FontGlobalScale);
                    if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Hammer, ref width))
                    {
                        MediatorService.Publish(new ToggleGenericWindowMessage(typeof(CraftsWindow)));
                    }

                    ImGuiUtil.HoverTooltip("打开制作窗口。");

                    var totalItems =  itemTable.RenderSearchResults.Count + " 件物品";

                    if (SelectedConfiguration != null && SelectedConfiguration.FilterType == FilterType.GameItemFilter)
                    {
                        totalItems =  itemTable.RenderSearchResults.Count + " 件物品";
                    }

                    if (SelectedConfiguration != null && SelectedConfiguration.FilterType == FilterType.HistoryFilter)
                    {
                        totalItems =  itemTable.RenderSearchResults.Count + " 条历史记录";
                    }

                    var calcTextSize = ImGui.CalcTextSize(totalItems);
                    width -= calcTextSize.X + 15;
                    ImGui.SetCursorPosX(width);
                    ImGuiService.VerticalCenter(totalItems);
                }
            }

            return filterConfiguration.Key;
        }

        private void SaveCallback(FilterTable filterTable, bool arg1, string arg2)
        {
            if (arg1)
            {
                filterTable.ExportToCsv(arg2);
            }
        }

        public override Vector2? DefaultSize => new Vector2(600, 500);
        public override Vector2? MaxSize => new Vector2(1500, 1500);
        public override Vector2? MinSize => new Vector2(200, 200);
    }
}
