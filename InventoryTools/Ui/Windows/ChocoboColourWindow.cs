using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.Shared.Extensions;
using DalaMock.Host.Mediator;
using DalaMock.Shared.Interfaces;
using Dalamud.Bindings.ImGui;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using InventoryTools.Logic;
using InventoryTools.Mediator;
using InventoryTools.Services;
using InventoryTools.Services.Interfaces;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Ui
{
    public class ChocoboColourWindow : GenericWindow, IMenuWindow
    {
        private const uint HanLemonItemId = 8163u;
        private const string ChocoboGuideUrl = "https://ffxiv.pf-n.co/chocobo-color";

        private readonly ExcelSheet<Item> _itemSheet;
        private readonly IClientState _clientState;
        private readonly IClipboardService _clipboardService;
        private readonly IListService _listService;
        private readonly IGameInteropService _gameInteropService;
        private readonly ChocoboColourSolver _colourSolver;
        private readonly IFont _font;

        private List<StainInfo> _allStains = new();
        private List<(byte Shade, string Label, List<StainInfo> Stains)> _stainsByShade = new();

        private readonly record struct FruitDisplay(ChocoboColourSolver.ChocoboFruit Fruit, uint IconId);

        private readonly List<FruitDisplay> _fruitDisplays = new();

        private uint _currentStainId = ChocoboColourSolver.DefaultStainId;
        private uint _targetStainId;
        private string _targetFilter = string.Empty;
        private string _currentFilter = string.Empty;

        private List<ChocoboColourSolver.ChocoboFruit>? _solverResult;
        private bool _pathNotFound;
        private string _statusMessage = string.Empty;
        private bool _openFooterMenu;
        private bool _openConfirmReset;
        private List<ChocoboColourSolver.ChocoboFruit> _lockedFruits = new();

        private bool IsLocked => Configuration.ChocoboLockedFruitIds.Count > 0;

        public ChocoboColourWindow(
            ILogger<ChocoboColourWindow> logger,
            MediatorService mediator,
            ImGuiService imGuiService,
            InventoryToolsConfiguration configuration,
            ExcelSheet<Item> itemSheet,
            IClientState clientState,
            IClipboardService clipboardService,
            IListService listService,
            IGameInteropService gameInteropService,
            ChocoboColourSolver colourSolver,
            IFont font,
            string name = "")
            : base(logger, mediator, imGuiService, configuration, string.IsNullOrEmpty(name) ? "陆行鸟颜色计算器" : name)
        {
            _itemSheet = itemSheet;
            _clientState = clientState;
            _clipboardService = clipboardService;
            _listService = listService;
            _gameInteropService = gameInteropService;
            _colourSolver = colourSolver;
            _font = font;
        }

        public override void Initialize()
        {
            Key = "chocoboColour";

            _allStains = _colourSolver.GetStains();

            RebuildShadeGroups(_allStains);

            foreach (var fruit in _colourSolver.Fruits)
            {
                var row = _itemSheet.GetRowOrDefault(fruit.ItemId);
                uint iconId = row.HasValue ? (uint)row.Value.Icon : 0u;
                _fruitDisplays.Add(new FruitDisplay(fruit, iconId));
            }

            LoadLockedState();
        }

        private void RebuildShadeGroups(IEnumerable<StainInfo> stains)
        {
            _stainsByShade = stains
                .GroupBy(s => s.Shade)
                .OrderBy(g => g.Key)
                .Select(g => (g.Key, GetShadeName(g.Key), g.ToList()))
                .ToList();
        }

        private static string GetShadeName(byte shade) => shade switch
        {
            2 => "白色 / 灰色",
            3 => "红色",
            4 => "粉色 / 红色",
            5 => "橙色 / 棕色",
            6 => "黄色",
            7 => "绿色",
            8 => "蓝色",
            9 => "紫色",
            10 => "金属色",
            _ => "色度 " + shade,
        };

        public override string GenericKey => "chocobo_colour";
        public override string GenericName => "陆行鸟颜色计算器";
        public override bool DestroyOnClose => false;
        public override bool SaveState => true;
        public override Vector2? DefaultSize => new(950, 560);
        public override Vector2? MaxSize => new(1400, 1000);
        public override Vector2? MinSize => new(600, 400);

        public override FilterConfiguration? SelectedConfiguration => null;

        public override void Invalidate()
        {
        }

        public override void DrawWindow()
        {
            if (_openFooterMenu)
            {
                ImGui.OpenPopup("ccFooterMenu");
                _openFooterMenu = false;
            }

            if (_openConfirmReset)
            {
                ImGui.OpenPopup("ccConfirmReset");
                _openConfirmReset = false;
            }

            DrawFooterMenuPopup();
            DrawConfirmResetPopup();

            float spacing = ImGui.GetStyle().ItemSpacing.X;
            float totalWidth = ImGui.GetContentRegionAvail().X;
            float panelHeight = ImGui.GetContentRegionAvail().Y;
            float panelWidth = (totalWidth - spacing * 2f) / 3f;

            using (var child = ImRaii.Child("##ccCurrent", new Vector2(panelWidth, panelHeight), true))
            {
                if (child.Success)
                {
                    DrawCurrentPanel(panelWidth);
                }
            }

            ImGui.SameLine();

            using (var child = ImRaii.Child("##ccTarget", new Vector2(panelWidth, panelHeight), true))
            {
                if (child.Success)
                {
                    DrawTargetPanel(panelWidth);
                }
            }

            ImGui.SameLine();

            using (var child = ImRaii.Child("##ccResult", new Vector2(panelWidth, panelHeight), true))
            {
                if (child.Success)
                {
                    DrawResultPanel();
                }
            }
        }

        private void DrawCurrentPanel(float panelWidth)
        {
            ImGui.TextUnformatted("当前颜色");
            ImGui.Separator();
            ImGui.Spacing();

            DrawColourSwatch(_currentStainId, "ccCurSwatch", 48f);
            ImGui.SameLine();
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 14f);
            ImGui.TextUnformatted(GetStainName(_currentStainId));

            ImGui.Spacing();

            var disabled = ImRaii.Disabled(_clientState.IsLoggedIn);

            if (ImGui.Button("从陆行鸟复制"))
            {
                var stainId = _gameInteropService.GetChocoboStainId();
                if (stainId.HasValue && stainId.Value > 0)
                {
                    _currentStainId = stainId.Value;
                    _solverResult = null;
                    _statusMessage = string.Empty;
                }
                else
                {
                    _statusMessage = stainId is null
                        ? "未登录。"
                        : "未召唤伙伴（颜色0时默认沙黄色）。";
                }
            }

            disabled.Dispose();
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
            {
                using (ImRaii.Tooltip())
                {
                    ImGui.TextUnformatted("请先登录。");
                }
            }

            if (!string.IsNullOrEmpty(_statusMessage))
            {
                ImGui.Spacing();
                ImGui.TextWrapped(_statusMessage);
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.TextUnformatted("或手动选择：");
            ImGui.Spacing();

            ImGui.SetNextItemWidth(-1);
            ImGui.InputText("##ccCurFilter", ref _currentFilter, 64);
            ImGui.Spacing();

            var curFiltered = string.IsNullOrWhiteSpace(_currentFilter)
                ? _allStains
                : _allStains.Where(s => s.Name.Contains(_currentFilter, StringComparison.OrdinalIgnoreCase)).ToList();

            using (var scroll = ImRaii.Child("##ccCurGrid", new Vector2(-1, -1), false,
                       ImGuiWindowFlags.HorizontalScrollbar))
            {
                if (scroll.Success)
                {
                    DrawStainGrid(curFiltered, ref _currentStainId, "ccCur", panelWidth - 20f);
                }
            }
        }

        private void DrawTargetPanel(float panelWidth)
        {
            ImGui.TextUnformatted("目标颜色");
            ImGui.Separator();
            ImGui.Spacing();

            if (_targetStainId == 0)
            {
                ImGui.TextUnformatted("（未选择）");
            }
            else
            {
                DrawColourSwatch(_targetStainId, "ccTgtSwatch", 48f);
                ImGui.SameLine();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 14f);
                ImGui.TextUnformatted(GetStainName(_targetStainId));
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.TextUnformatted("选择目标颜色：");
            ImGui.Spacing();

            ImGui.SetNextItemWidth(-1);
            ImGui.InputText("##ccTgtFilter", ref _targetFilter, 64);

            ImGui.Spacing();

            using var scroll = ImRaii.Child("##ccTgtGrid", new Vector2(-1, -1), false,
                ImGuiWindowFlags.HorizontalScrollbar);
            if (!scroll.Success) return;
            if (string.IsNullOrWhiteSpace(_targetFilter))
            {
                foreach (var (shade, label, stains) in _stainsByShade)
                {
                    bool open = ImGui.CollapsingHeader(string.Format("{0}##shade{1}", label, shade),
                        ImGuiTreeNodeFlags.DefaultOpen);
                    if (open)
                    {
                        ImGui.Indent(4f);
                        DrawStainGrid(stains, ref _targetStainId, $"ccTgt{shade}",
                            panelWidth - 30f);
                        ImGui.Unindent(4f);
                        ImGui.Spacing();
                    }
                }
            }
            else
            {
                var filtered = _allStains
                    .Where(s => s.Name.Contains(_targetFilter, StringComparison.OrdinalIgnoreCase))
                    .ToList();
                DrawStainGrid(filtered, ref _targetStainId, "ccTgtF", panelWidth - 20f);
            }
        }

        private void DrawResultPanel()
        {
            ImGui.TextUnformatted("果实序列");
            ImGui.SameLine();
            ImGuiService.HelpMarker(
                "水果序列基于 RGB 颜色计算估算。\n" +
                "由于游戏内取整，结果偶尔会有差异\n" +
                "如果未达到目标颜色，可能偏差一两个水果。\n" +
                "喂食汉柠檬重置为沙黄色并重试。");
            ImGui.Separator();
            ImGui.Spacing();

            float footerHeight = ImGui.GetFrameHeightWithSpacing() + ImGui.GetStyle().ItemSpacing.Y;
            using (var body = ImRaii.Child("##ccResultBody", new Vector2(-1, -footerHeight), false))
            {
                if (body.Success)
                {
                    DrawResultBody();
                }
            }

            ImGui.Separator();
            DrawResultFooter();
        }

        private void DrawResultBody()
        {
            if (IsLocked)
            {
                DrawTrackingBody();
                return;
            }

            DrawColourSwatch(_currentStainId, "ccResFrom", 20f);
            ImGui.SameLine();
            ImGui.TextUnformatted("→");
            ImGui.SameLine();
            if (_targetStainId == 0)
            {
                ImGui.TextDisabled("（无目标）");
            }
            else
            {
                DrawColourSwatch(_targetStainId, "ccResTo", 20f);
                ImGui.SameLine();
                ImGui.TextUnformatted(GetStainName(_targetStainId));
            }

            ImGui.Spacing();

            bool canLock = _solverResult is { Count: > 0 };
            if (!canLock)
            {
                ImGui.BeginDisabled();
            }

            if (ImGui.Button("锁定##ccLockIn"))
            {
                LockInSolverResult();
            }

            if (!canLock)
            {
                ImGui.EndDisabled();
            }

            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
            {
                using (ImRaii.Tooltip())
                {
                    ImGui.TextUnformatted("锁定此序列以追踪喂食进度。");
                    ImGui.TextUnformatted("每喂食一个果实，就勾选它。");
                }
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            if (_pathNotFound)
            {
                ImGui.TextColoredWrapped(new Vector4(1f, 0.4f, 0.4f, 1f),
                    "未找到这两种颜色之间的路径。它们可能是无法通过喂食果实互相到达的罕见颜色对之一。");
                return;
            }

            if (_currentStainId == _targetStainId)
            {
                ImGui.TextColoredWrapped(new Vector4(0.4f, 1f, 0.4f, 1f),
                    "已处于目标颜色！");
                return;
            }

            if (_solverResult == null)
            {
                ImGui.TextDisabled("选择目标颜色以生成果实序列。");
                return;
            }

            if (_solverResult.Count == 0)
            {
                ImGui.TextDisabled("无需果实（已达目标颜色）。");
                return;
            }

            ImGui.TextUnformatted(string.Format("需要喂食的果实：{0}", _solverResult.Count));
            ImGui.Spacing();

            using (var scroll = ImRaii.Child("##ccFruits", new Vector2(-1, -60f), false))
            {
                if (scroll.Success)
                {
                    DrawFruitList();
                }
            }

            ImGui.Spacing();

            if (ImGui.Button("加入列表##ccAddList"))
            {
                ImGui.OpenPopup("ccAddToList");
            }

            DrawAddToListPopup();

            ImGui.SameLine();

            if (ImGui.Button("复制到剪贴板##ccCopy"))
            {
                CopyResultToClipboard();
            }

            ImGui.SameLine();

            if (ImGui.Button("清除##ccClear"))
            {
                _solverResult = null;
                _pathNotFound = false;
            }
        }

        private void DrawResultFooter()
        {
            float qCursorX = ImGui.GetCursorPosX();
            if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.QuestionCircle, ref qCursorX,
                    "汉柠檬 / 指南"))
            {
                _openFooterMenu = true;
            }
        }

        private void DrawFooterMenuPopup()
        {
            using var popup = ImRaii.Popup("ccFooterMenu");
            if (!popup.Success)
            {
                return;
            }

            using (var hanLemonMenu = ImRaii.Menu("汉柠檬"))
            {
                if (ImGui.IsItemHovered())
                {
                    using (ImRaii.Tooltip())
                    {
                        ImGui.TextUnformatted("喂食陆行鸟以重置颜色");
                        ImGui.TextUnformatted("返回沙黄色（默认），");
                        ImGui.TextUnformatted("以便你重新开始染色。");
                    }
                }

                if (hanLemonMenu)
                {
                    ImGui.TextDisabled("将陆行鸟颜色重置为沙黄色。");
                    ImGui.TextDisabled("染色前先喂食以重新开始。");
                    ImGui.Separator();

                    var items = new List<(uint ItemId, uint Quantity)> { (HanLemonItemId, 1u) };

                    var craftLists = _listService.Lists
                        .Where(c => c.FilterType == FilterType.CraftFilter && !c.CraftListDefault).ToArray();
                    if (craftLists.Length != 0)
                    {
                        using var craftMenu = ImRaii.Menu("添加到制作列表");
                        if (craftMenu)
                        {
                            foreach (var filter in craftLists)
                            {
                                if (!ImGui.Selectable(filter.Name))
                                {
                                    continue;
                                }

                                AddFruitsToCraftList(filter, items);
                            }
                        }
                    }

                    if (ImGui.Selectable("添加到新制作列表"))
                    {
                        AddFruitsToCraftList(_listService.AddNewCraftList(), items);
                    }

                    if (ImGui.Selectable("添加到新制作列表（临时）"))
                    {
                        AddFruitsToCraftList(_listService.AddNewCraftList(null, true), items);
                    }

                    ImGui.Separator();

                    var curatedLists = _listService.Lists
                        .Where(c => c.FilterType == FilterType.CuratedList).ToArray();
                    if (curatedLists.Length != 0)
                    {
                        using var curatedMenu = ImRaii.Menu("添加到精选列表");
                        if (curatedMenu)
                        {
                            foreach (var filter in curatedLists)
                            {
                                if (!ImGui.MenuItem(filter.Name))
                                {
                                    continue;
                                }

                                AddFruitsToCuratedList(filter, items);
                            }
                        }
                    }

                    if (ImGui.Selectable("添加到新精选列表"))
                    {
                        AddFruitsToCuratedList(_listService.AddNewCuratedList(), items);
                    }
                }
            }

            ImGui.Separator();

            if (ImGui.MenuItem("了解更多"))
            {
                ChocoboGuideUrl.OpenBrowser();
            }

            if (ImGui.IsItemHovered())
            {
                using (ImRaii.Tooltip())
                {
                    ImGui.TextUnformatted("陆行鸟颜色数据由 Lulu 提供。");
                    ImGui.TextDisabled(ChocoboGuideUrl);
                }
            }
        }

        private void DrawAddToListPopup()
        {
            using var popup = ImRaii.Popup("ccAddToList");
            if (!popup.Success)
            {
                return;
            }

            var fruits = GetRequiredFruits();
            if (fruits.Count == 0)
            {
                ImGui.TextDisabled("没有可添加的果实。");
                return;
            }

            var craftLists = _listService.Lists
                .Where(c => c.FilterType == FilterType.CraftFilter && !c.CraftListDefault).ToArray();
            if (craftLists.Length != 0)
            {
                using var menu = ImRaii.Menu("添加到制作列表");
                if (menu)
                {
                    foreach (var filter in craftLists)
                    {
                        if (!ImGui.Selectable(filter.Name))
                        {
                            continue;
                        }

                        AddFruitsToCraftList(filter, fruits);
                    }
                }
            }

            if (ImGui.Selectable("添加到新制作列表"))
            {
                AddFruitsToCraftList(_listService.AddNewCraftList(), fruits);
            }

            if (ImGui.Selectable("添加到新制作列表（临时）"))
            {
                AddFruitsToCraftList(_listService.AddNewCraftList(null, true), fruits);
            }

            ImGui.Separator();

            var curatedLists = _listService.Lists
                .Where(c => c.FilterType == FilterType.CuratedList).ToArray();
            if (curatedLists.Length != 0)
            {
                using var menu = ImRaii.Menu("添加到精选列表");
                if (menu)
                {
                    foreach (var filter in curatedLists)
                    {
                        if (!ImGui.MenuItem(filter.Name))
                        {
                            continue;
                        }

                        AddFruitsToCuratedList(filter, fruits);
                    }
                }
            }

            if (ImGui.Selectable("添加到新精选列表"))
            {
                AddFruitsToCuratedList(_listService.AddNewCuratedList(), fruits);
            }
        }

        private List<ChocoboColourSolver.ChocoboFruit> GetActiveFruits()
        {
            if (IsLocked)
            {
                return _lockedFruits;
            }

            return _solverResult ?? new List<ChocoboColourSolver.ChocoboFruit>();
        }

        private List<(uint ItemId, uint Quantity)> GetRequiredFruits()
        {
            var fruits = GetActiveFruits();
            if (fruits.Count == 0)
            {
                return new();
            }

            return fruits
                .GroupBy(f => f.ItemId)
                .Select(g => (g.Key, (uint)g.Count()))
                .ToList();
        }

        private void AddFruitsToCraftList(FilterConfiguration filter, List<(uint ItemId, uint Quantity)> fruits)
        {
            foreach (var (itemId, quantity) in fruits)
            {
                filter.CraftList.AddCraftItem(itemId, quantity);
            }

            filter.NeedsRefresh = true;
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(CraftsWindow)));
            MediatorService.Publish(new FocusListMessage(typeof(CraftsWindow), filter));
        }

        private void AddFruitsToCuratedList(FilterConfiguration filter, List<(uint ItemId, uint Quantity)> fruits)
        {
            foreach (var (itemId, quantity) in fruits)
            {
                filter.AddCuratedItem(new CuratedItem(itemId, quantity, InventoryItem.ItemFlags.None));
            }

            filter.NeedsRefresh = true;
            MediatorService.Publish(new FocusListMessage(typeof(FiltersWindow), filter));
        }

        private void DrawFruitList()
        {
            const float iconSize = 20f;
            var iconVec = new Vector2(iconSize);

            for (int i = 0; i < _solverResult!.Count; i++)
            {
                var fruit = _solverResult[i];
                var display = _fruitDisplays.FirstOrDefault(fd => fd.Fruit == fruit);

                ImGui.TextUnformatted($"{i + 1,3}.");
                ImGui.SameLine();

                if (display.IconId != 0)
                {
                    var tex = ImGuiService.GetIconTexture(display.IconId);
                    ImGui.Image(tex.Handle, iconVec);
                    ImGui.SameLine();
                }

                ImGui.TextUnformatted(LocalizationService.Ui(fruit.Name));
            }
        }

        private void DrawStainGrid(
            IEnumerable<StainInfo> stains,
            ref uint selectedId,
            string idPrefix,
            float availWidth)
        {
            const float baseSize = 22f;
            float swatchSize = baseSize * ImGui.GetIO().FontGlobalScale;
            float spacing = ImGui.GetStyle().ItemSpacing.X;
            int perRow = Math.Max(1, (int)((availWidth + spacing) / (swatchSize + spacing)));

            int col = 0;
            foreach (var stain in stains)
            {
                bool isSelected = stain.RowId == selectedId;

                if (ImGui.ColorButton(
                        $"##{idPrefix}_{stain.RowId}",
                        stain.AsVec4,
                        ImGuiColorEditFlags.NoTooltip,
                        new Vector2(swatchSize)))
                {
                    selectedId = stain.RowId;
                    _solverResult = null;
                    _pathNotFound = false;
                    RunSolver();
                }

                if (isSelected)
                {
                    var min = ImGui.GetItemRectMin();
                    var max = ImGui.GetItemRectMax();
                    ImGui.GetWindowDrawList().AddRect(
                        min - new Vector2(1f),
                        max + new Vector2(1f),
                        0xFFFFFF00u, 2f, 0, 2f);
                }

                if (ImGui.IsItemHovered())
                {
                    using (ImRaii.Tooltip())
                    {
                        ImGui.TextUnformatted(stain.Name);
                    }
                }

                col++;
                if (col < perRow)
                {
                    ImGui.SameLine();
                }
                else
                {
                    col = 0;
                }
            }
        }

        private void DrawColourSwatch(uint stainId, string idPrefix, float size)
        {
            var stain = _allStains.FirstOrDefault(s => s.RowId == stainId);
            var colour = stain.RowId != 0 ? stain.AsVec4 : new Vector4(0.5f, 0.5f, 0.5f, 1f);
            ImGui.ColorButton($"##{idPrefix}_preview", colour,
                ImGuiColorEditFlags.NoTooltip,
                new Vector2(size));
        }

        private string GetStainName(uint stainId)
        {
            var stain = _allStains.FirstOrDefault(s => s.RowId == stainId);
            if (stain.RowId != 0)
            {
                return stain.Name;
            }
            return stainId == ChocoboColourSolver.DefaultStainId ? "沙黄色（默认）" : "（未知）";
        }

        private void RunSolver()
        {
            if (IsLocked)
            {
                return;
            }

            _solverResult = null;
            _pathNotFound = false;
            _statusMessage = string.Empty;

            var curStain = _allStains.FirstOrDefault(s => s.RowId == _currentStainId);
            var tgtStain = _allStains.FirstOrDefault(s => s.RowId == _targetStainId);

            if (curStain.RowId == 0)
            {
                var fallback = _colourSolver.GetDefaultStain();
                if (fallback.HasValue)
                {
                    curStain = fallback.Value;
                }
            }

            if (tgtStain.RowId == 0 || curStain.RowId == 0)
            {
                return;
            }

            var current = (curStain.R, curStain.G, curStain.B);
            var target = (tgtStain.R, tgtStain.G, tgtStain.B);

            _solverResult = _colourSolver.Solve(current, target);

            var reached = current;
            foreach (var fruit in _solverResult)
            {
                reached = _colourSolver.Apply(reached, fruit);
            }

            var nearestStain = _allStains
                .OrderBy(s => _colourSolver.EuclideanDistance((s.R, s.G, s.B), reached))
                .FirstOrDefault();

            if (nearestStain.RowId != _targetStainId)
            {
                _pathNotFound = true;
                _solverResult = null;
            }
        }

        private void CopyResultToClipboard()
        {
            var fruits = GetActiveFruits();
            if (fruits.Count == 0)
            {
                return;
            }

            uint fromId = IsLocked ? Configuration.ChocoboLockedCurrentStainId : _currentStainId;
            uint toId = IsLocked ? Configuration.ChocoboLockedTargetStainId : _targetStainId;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine(string.Format("陆行鸟羽毛颜色：{0} -> {1}", GetStainName(fromId),
                GetStainName(toId)));
            for (int i = 0; i < fruits.Count; i++)
            {
                sb.AppendLine($"{i + 1}. {fruits[i].Name}");
            }

            _clipboardService.CopyToClipboard(sb.ToString().TrimEnd());
        }

        private void LoadLockedState()
        {
            _lockedFruits = Configuration.ChocoboLockedFruitIds
                .Select(id => _colourSolver.Fruits.FirstOrDefault(f => f.ItemId == id))
                .OfType<ChocoboColourSolver.ChocoboFruit>()
                .ToList();
        }

        private void LockInSolverResult()
        {
            if (_solverResult == null || _solverResult.Count == 0)
            {
                return;
            }

            Configuration.ChocoboLockedCurrentStainId = _currentStainId;
            Configuration.ChocoboLockedTargetStainId = _targetStainId;
            Configuration.ChocoboLockedFruitIds = _solverResult.Select(f => f.ItemId).ToList();
            Configuration.ChocoboFruitsCheckedCount = 0;
            Configuration.IsDirty = true;
            LoadLockedState();
            _solverResult = null;
        }

        private void DrawTrackingBody()
        {
            int checkedCount = Math.Clamp(Configuration.ChocoboFruitsCheckedCount, 0, _lockedFruits.Count);

            DrawColourSwatch(Configuration.ChocoboLockedCurrentStainId, "ccLockedFrom", 20f);
            ImGui.SameLine();
            ImGui.TextUnformatted("→");
            ImGui.SameLine();
            DrawColourSwatch(Configuration.ChocoboLockedTargetStainId, "ccLockedTo", 20f);
            ImGui.SameLine();
            ImGui.TextUnformatted(GetStainName(Configuration.ChocoboLockedTargetStainId));

            ImGui.Spacing();

            if (checkedCount >= _lockedFruits.Count)
            {
                ImGui.TextColoredWrapped(new Vector4(0.4f, 1f, 0.4f, 1f),
                    "所有果实已喂食！请检查陆行鸟的颜色。");
            }
            else
            {
                ImGui.TextUnformatted(string.Format("进度：已喂食 {0} / {1} 个果实", checkedCount, _lockedFruits.Count));
            }

            ImGui.Spacing();
            ImGui.Separator();
            ImGui.Spacing();

            float footerHeight = ImGui.GetFrameHeightWithSpacing() + ImGui.GetStyle().ItemSpacing.Y;
            using (var scroll = ImRaii.Child("##ccTrackFruits", new Vector2(-1, -footerHeight), false))
            {
                if (scroll.Success)
                {
                    DrawTrackingChecklist(checkedCount);
                }
            }

            ImGui.Separator();

            if (ImGui.Button("重置##ccReset"))
            {
                _openConfirmReset = true;
            }

            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled))
            {
                using (ImRaii.Tooltip())
                {
                    ImGui.TextUnformatted("清除锁定的序列并重置所有喂食进度。");
                }
            }

            ImGui.SameLine();

            if (ImGui.Button("加入列表##ccTrackAddList"))
            {
                ImGui.OpenPopup("ccAddToList");
            }

            DrawAddToListPopup();

            ImGui.SameLine();

            if (ImGui.Button("复制到剪贴板##ccTrackCopy"))
            {
                CopyResultToClipboard();
            }
        }

        private void DrawTrackingChecklist(int checkedCount)
        {
            const float iconSize = 20f;
            var iconVec = new Vector2(iconSize);

            for (int i = 0; i < _lockedFruits.Count; i++)
            {
                var fruit = _lockedFruits[i];
                bool isChecked = i < checkedCount;
                bool isEnabled = i == checkedCount;

                if (!isEnabled)
                {
                    ImGui.BeginDisabled();
                }

                if (ImGui.Checkbox(string.Format("##{0}ccTrackChk", i), ref isChecked))
                {
                    Configuration.ChocoboFruitsCheckedCount = isChecked ? i + 1 : i;
                    Configuration.IsDirty = true;
                }

                if (!isEnabled)
                {
                    ImGui.EndDisabled();
                }

                ImGui.SameLine();
                ImGui.TextUnformatted($"{i + 1,3}.");
                ImGui.SameLine();

                var display = _fruitDisplays.FirstOrDefault(fd => fd.Fruit == fruit);
                if (display.IconId != 0)
                {
                    var tex = ImGuiService.GetIconTexture(display.IconId);
                    ImGui.Image(tex.Handle, iconVec);
                    ImGui.SameLine();
                }

                ImGui.TextUnformatted(LocalizationService.Ui(fruit.Name));
            }
        }

        private void DrawConfirmResetPopup()
        {
            using var popup = ImRaii.Popup("ccConfirmReset");
            if (!popup.Success)
            {
                return;
            }

            ImGui.TextUnformatted("重置追踪进度？");
            ImGui.TextDisabled("这将清除锁定的颜色和所有进度。");
            ImGui.Spacing();

            if (ImGui.Button("重置##ccResetYes"))
            {
                Configuration.ChocoboLockedFruitIds = new List<uint>();
                Configuration.ChocoboLockedCurrentStainId = 0;
                Configuration.ChocoboLockedTargetStainId = 0;
                Configuration.ChocoboFruitsCheckedCount = 0;
                Configuration.IsDirty = true;
                _lockedFruits.Clear();
                ImGui.CloseCurrentPopup();
            }

            ImGui.SameLine();

            if (ImGui.Button("取消##ccResetCancel"))
            {
                ImGui.CloseCurrentPopup();
            }
        }
    }
}
