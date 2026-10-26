using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using Humanizer;
using InventoryTools.Extensions;
using InventoryTools.Logic;
using InventoryTools.Mediator;
using InventoryTools.Services;
using InventoryTools.Services.Interfaces;
using LuminaSupplemental.Excel.Model;
using Microsoft.Extensions.Logging;
using OtterGui;
using InventoryTools.Localization;

namespace InventoryTools.Ui
{
    class BNpcWindow : UintWindow
    {
        private readonly IChatUtilities _chatUtilities;
        private readonly IClipboardService _clipboardService;
        private readonly BNpcNameSheet _bNpcNameSheet;
        private readonly TerritoryTypeSheet _territoryTypeSheet;
        private readonly ItemSheet _itemSheet;
        private readonly IListService _listService;
        private readonly EquipSlot[] _equipSlots = [EquipSlot.MainHand, EquipSlot.OffHand, EquipSlot.Head, EquipSlot.Gloves, EquipSlot.Body, EquipSlot.Legs, EquipSlot.Feet, EquipSlot.FingerL, EquipSlot.FingerR];

        public BNpcWindow(ILogger<BNpcWindow> logger,
            MediatorService mediator,
            ImGuiService imGuiService,
            InventoryToolsConfiguration configuration,
            IChatUtilities chatUtilities,
            IClipboardService clipboardService,
            BNpcNameSheet bNpcNameSheet,
            TerritoryTypeSheet territoryTypeSheet,
            ItemSheet itemSheet,
            IListService listService,
            string name = "") : base(logger, mediator, imGuiService, configuration, string.IsNullOrEmpty(name) ? "怪物窗口" : name)
        {
            _chatUtilities = chatUtilities;
            _clipboardService = clipboardService;
            _bNpcNameSheet = bNpcNameSheet;
            _territoryTypeSheet = territoryTypeSheet;
            _itemSheet = itemSheet;
            _listService = listService;
        }
        public override void Initialize(uint bNpcId)
        {
            base.Initialize(bNpcId);
            Flags = ImGuiWindowFlags.NoSavedSettings;
            _bNpcId = bNpcId;
            if (bNpc != null)
            {
                WindowName = bNpc.Base.Singular.ExtractText().ToTitleCase() + "##" + bNpcId;
                Key = "bNpc_" + bNpcId;
                _mobDrops = bNpc.MobDrops;
                _mobSpawns = bNpc.MobSpawnPositions;
            }
            else
            {
                WindowName = "未知怪物";
            }
        }

        public override bool SaveState => false;
        private uint _bNpcId;
        private List<MobDrop>? _mobDrops;
        private List<MobSpawnPosition>? _mobSpawns;

        private BNpcNameRow? bNpc => _bNpcNameSheet.GetRowOrDefault(_bNpcId);
        public override string GenericName => "怪物";
        public override bool DestroyOnClose => true;
        public override void DrawWindow()
        {
            if (bNpc == null)
            {
                ImGui.TextUnformatted("战斗NPC ID：" + _bNpcId + "未找到。");
            }
            else
            {
                ImGui.Text("类型：" + string.Join(", ", bNpc.MobTypes.Select(c => LocalizationService.Ui(c.ToString()))));

                if (bNpc.NotoriousMonster != null)
                {
                    ImGui.Text("等级：" + bNpc.NotoriousMonster?.RankFormatted());
                }

                var garlandId = bNpc.GarlandToolsId;
                if (garlandId != null)
                {
                    if (ImGui.ImageButton(ImGuiService.GetImageTexture("garlandtools").Handle,
                            new Vector2(32, 32) * ImGui.GetIO().FontGlobalScale))
                    {
                        $"https://www.garlandtools.org/db/#mob/{garlandId}".OpenBrowser();
                    }

                    ImGuiUtil.HoverTooltip("在 Garland Tools 中打开");
                    ImGui.SameLine();
                }

                if (ImGui.ImageButton(ImGuiService.GetImageTexture("teamcraft").Handle,
                        new Vector2(32, 32) * ImGui.GetIO().FontGlobalScale))
                {
                    $"https://ffxivteamcraft.com/db/en/mob/{_bNpcId}".OpenBrowser();
                }
                ImGuiUtil.HoverTooltip("在 Teamcraft 中打开");

                ImGui.Separator();


                if (_mobDrops != null && ImGui.CollapsingHeader("掉落（" + _mobDrops.Count + ")", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.CollapsingHeader))
                {
                    ImGuiStylePtr style = ImGui.GetStyle();
                    float windowVisibleX2 = ImGui.GetWindowPos().X + ImGui.GetWindowContentRegionMax().X;
                    for (var index = 0; index < _mobDrops.Count; index++)
                    {
                        ImGui.PushID("Drop"+index);
                        var drop = _mobDrops[index];
                        var listingCount = 0;

                        if (drop.Item.IsValid)
                        {
                            var useIcon = ImGuiService.GetIconTexture(drop.Item.Value.Icon);
                            if (ImGui.ImageButton(useIcon.Handle,
                                    new Vector2(32, 32) * ImGui.GetIO().FontGlobalScale,
                                    new(0, 0), new(1, 1),
                                    0))
                            {
                                MediatorService.Publish(new OpenUintWindowMessage(typeof(ItemWindow), drop.Item.RowId));
                            }

                            if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled &
                                                    ImGuiHoveredFlags.AllowWhenOverlapped &
                                                    ImGuiHoveredFlags.AllowWhenBlockedByPopup &
                                                    ImGuiHoveredFlags
                                                        .AllowWhenBlockedByActiveItem &
                                                    ImGuiHoveredFlags.AnyWindow) &&
                                ImGui.IsMouseReleased(ImGuiMouseButton.Right))
                            {
                                ImGui.OpenPopup("RightClickUse" + drop.Item.RowId);
                            }

                            using (var popup = ImRaii.Popup("RightClickUse"+ drop.Item.RowId))
                            {
                                if (popup)
                                {
                                    MediatorService.Publish(
                                        ImGuiService.ImGuiMenuService.DrawRightClickPopup(
                                            _itemSheet.GetRow(drop.Item.RowId)));
                                }
                            }

                            float lastButtonX2 = ImGui.GetItemRectMax().X;
                            float nextButtonX2 = lastButtonX2 + style.ItemSpacing.X + 32;
                            ImGuiUtil.HoverTooltip(_itemSheet.GetRow(drop.Item.RowId).NameString);
                            if (listingCount < _mobDrops.Count && nextButtonX2 < windowVisibleX2)
                            {
                                ImGui.SameLine();
                            }
                        }
                    }
                }

                ImGui.NewLine();

                if (_mobSpawns != null && ImGui.CollapsingHeader("位置（" + _mobSpawns.Count + ")", ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.CollapsingHeader))
                {
                    ImGuiStylePtr style = ImGui.GetStyle();
                    float windowVisibleX2 = ImGui.GetWindowPos().X + ImGui.GetWindowContentRegionMax().X;
                    for (var index = 0; index < _mobSpawns.Count; index++)
                    {
                        ImGui.PushID("Location"+index);
                        var spawn = _mobSpawns[index];
                        var listingCount = 0;

                        var territory = _territoryTypeSheet
                            .GetRowOrDefault(spawn.TerritoryTypeId);
                        if (territory != null)
                        {
                            if (ImGui.ImageButton(ImGuiService.GetIconTexture(60561).Handle,
                                    new Vector2(32 * ImGui.GetIO().FontGlobalScale,32 * ImGui.GetIO().FontGlobalScale), new Vector2(0, 0),
                                    new Vector2(1, 1), 0))
                            {
                                _chatUtilities.PrintFullMapLink(
                                    new GenericMapLocation(spawn.Position.X, spawn.Position.Y,
                                        territory.Base.Map,
                                        territory.Base.PlaceName,
                                        territory.RowRef), bNpc.Base.Singular.ExtractText());
                            }

                            if (ImGui.IsItemHovered())
                            {
                                using var tt = ImRaii.Tooltip();
                                ImGui.TextUnformatted((territory.Base.PlaceName.ValueNullable?.Name.ExtractText() ?? "未知") + " - " +
                                                      spawn.Position.X +
                                                      " : " + spawn.Position.Y);
                            }
                            float lastButtonX2 = ImGui.GetItemRectMax().X;
                            float nextButtonX2 = lastButtonX2 + style.ItemSpacing.X + 32;
                            ImGuiUtil.HoverTooltip(bNpc.Base.Singular.ExtractText());
                            if (listingCount < _mobSpawns.Count && nextButtonX2 < windowVisibleX2)
                            {
                                ImGui.SameLine();
                            }
                        }
                    }
                }

                ImGui.NewLine();
                var firstBase = bNpc.RelatedBases.FirstOrDefault();
                if (firstBase != null && firstBase.GetRelatedItems().Count != 0)
                {
                    if (ImGui.CollapsingHeader("共享模型", ImGuiTreeNodeFlags.DefaultOpen))
                    {
                        using (ImRaii.PushIndent())
                        {
                            foreach (var slot in _equipSlots)
                            {
                                DrawSharedModels(slot);
                            }
                        }
                    }
                }


#if DEBUG
                ImGui.NewLine();
                if (ImGui.CollapsingHeader("调试"))
                {
                    ImGui.TextUnformatted("战斗NPC ID：" + _bNpcId);
                    if (ImGui.Button("复制"))
                    {
                        _clipboardService.CopyToClipboard(_bNpcId.ToString());
                    }

                    Utils.PrintOutObject(bNpc, 0, new List<string>());
                }
                #endif

                using (var popup = ImRaii.Popup("StealLookPopup"))
                {
                    if (popup)
                    {
                        if (firstBase != null)
                        {

                            var craftFilters =
                                _listService.Lists.Where(c =>
                                    c.FilterType == Logic.FilterType.CraftFilter && !c.CraftListDefault).ToArray();
                            if (craftFilters.Length != 0)
                            {
                                using var menu = ImRaii.Menu("添加到制作列表");
                                if (menu)
                                {
                                    foreach (var filter in craftFilters)
                                    {
                                        if (!ImGui.MenuItem(filter.Name)) continue;
                                        foreach (var slot in _equipSlots)
                                        {
                                            var relatedItem = firstBase.GetRelatedItems(slot).FirstOrDefault();
                                            if (relatedItem != null)
                                            {
                                                filter.CraftList.AddCraftItem(relatedItem.RowId);
                                            }
                                        }

                                        MediatorService.Publish(new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                        MediatorService.Publish(new FocusListMessage(typeof(CraftsWindow), filter));
                                        filter.NeedsRefresh = true;
                                    }
                                }
                            }

                            if (ImGui.MenuItem("添加到新制作列表"))
                            {
                                var filter = _listService.AddNewCraftList();
                                foreach (var slot in _equipSlots)
                                {
                                    var relatedItem = firstBase.GetRelatedItems(slot).FirstOrDefault();
                                    if (relatedItem != null)
                                    {
                                        filter.CraftList.AddCraftItem(relatedItem.RowId);
                                    }
                                }

                                MediatorService.Publish(new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                MediatorService.Publish(new FocusListMessage(typeof(CraftsWindow), filter));
                                filter.NeedsRefresh = true;
                            }

                            if (ImGui.MenuItem("添加到新制作列表（临时）"))
                            {
                                var filter = _listService.AddNewCraftList(null, true);
                                foreach (var slot in _equipSlots)
                                {
                                    var relatedItem = firstBase.GetRelatedItems(slot).FirstOrDefault();
                                    if (relatedItem != null)
                                    {
                                        filter.CraftList.AddCraftItem(relatedItem.RowId);
                                    }
                                }

                                MediatorService.Publish(new OpenGenericWindowMessage(typeof(CraftsWindow)));
                                MediatorService.Publish(new FocusListMessage(typeof(CraftsWindow), filter));
                                filter.NeedsRefresh = true;
                            }
                        }
                    }
                }
            }
        }

        public void DrawSharedModels(EquipSlot slot)
        {
            if (bNpc == null)
            {
                return;
            }

            var firstBase = bNpc.RelatedBases.FirstOrDefault();
            if (firstBase == null)
            {
                return;
            }
            var relatedItems = firstBase.GetRelatedItems(slot);
            if (relatedItems.Count == 0)
            {
                return;
            }
            if (ImGui.CollapsingHeader("共享模型 - " + EquipSlotName(slot) + " (" + relatedItems.Count + ")", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGuiStylePtr style = ImGui.GetStyle();
                float windowVisibleX2 = ImGui.GetWindowPos().X + ImGui.GetWindowContentRegionMax().X;
                for (var index = 0; index < relatedItems.Count; index++)
                {
                    using (ImRaii.PushId(index))
                    {
                        var sharedModel = relatedItems[index];
                        if (ImGui.ImageButton(ImGuiService.GetIconTexture(sharedModel.Icon).Handle,
                                new(32, 32)))
                        {
                            MediatorService.Publish(
                                new OpenUintWindowMessage(typeof(ItemWindow), sharedModel.RowId));
                        }

                        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled &
                                                ImGuiHoveredFlags.AllowWhenOverlapped &
                                                ImGuiHoveredFlags.AllowWhenBlockedByPopup &
                                                ImGuiHoveredFlags.AllowWhenBlockedByActiveItem &
                                                ImGuiHoveredFlags.AnyWindow) &&
                            ImGui.IsMouseReleased(ImGuiMouseButton.Right))
                        {
                            ImGui.OpenPopup("RightClick" + sharedModel.RowId);
                        }

                        using (var popup = ImRaii.Popup("RightClick" + sharedModel.RowId))
                        {
                            if (popup)
                            {
                                MediatorService.Publish(
                                    ImGuiService.ImGuiMenuService.DrawRightClickPopup(sharedModel));
                            }
                        }

                        float lastButtonX2 = ImGui.GetItemRectMax().X;
                        float nextButtonX2 = lastButtonX2 + style.ItemSpacing.X + 32;
                        ImGuiUtil.HoverTooltip(sharedModel.NameString);
                        if (index + 1 < relatedItems.Count && nextButtonX2 < windowVisibleX2)
                        {
                            ImGui.SameLine();
                        }
                    }
                }
            }
        }

        public override void Invalidate()
        {

        }

        public override FilterConfiguration? SelectedConfiguration => null;
        public override Vector2? DefaultSize { get; } = new Vector2(500, 800);
        public override Vector2? MaxSize => new (800, 1500);
        public override Vector2? MinSize => new (100, 100);

        public override bool SavePosition => true;

        public override string GenericKey => "bNpc";

        private static string EquipSlotName(EquipSlot slot) => slot switch
        {
            EquipSlot.MainHand => "主手",
            EquipSlot.OffHand => "副手",
            EquipSlot.Head => "头部",
            EquipSlot.Body => "身体",
            EquipSlot.Gloves => "手部",
            EquipSlot.Legs => "腿部",
            EquipSlot.Feet => "脚部",
            EquipSlot.Ears => "耳部",
            EquipSlot.Neck => "颈部",
            EquipSlot.Wrists => "腕部",
            EquipSlot.FingerR => "右手戒指",
            EquipSlot.FingerL => "左手戒指",
            EquipSlot.SoulCrystal => "特职水晶",
            _ => LocalizationService.Ui(slot.Humanize()),
        };
    }
}
