using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib.Enums;
using CriticalCommonLib.Extensions;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Bindings.ImGui;
using InventoryTools.Mediator;
using InventoryTools.Services;
using DalaMock.Shared.Interfaces;
using Dalamud.Interface;
using InventoryTools.Ui.Widgets;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Logging;
using ImGuiUtil = OtterGui.ImGuiUtil;
using InventoryTools.Localization;

namespace InventoryTools.Ui.Pages
{
    public class CharacterRetainerPage : Page
    {
        private readonly ICharacterMonitor _characterMonitor;
        private readonly IInventoryMonitor _inventoryMonitor;
        private readonly ExcelSheet<World> _worldSheet;
        private readonly IFont _font;

        public CharacterRetainerPage(ILogger<CharacterRetainerPage> logger, ImGuiService imGuiService, ICharacterMonitor characterMonitor, IInventoryMonitor inventoryMonitor, ExcelSheet<World> worldSheet, IFont font) : base(logger, imGuiService)
        {
            _characterMonitor = characterMonitor;
            _inventoryMonitor = inventoryMonitor;
            _worldSheet = worldSheet;
            _font = font;
        }
        private bool _isSeparator = false;
        public override void Initialize()
        {
        }

        public override string Name { get; } = "角色/雇员";

        private ulong _selectedCharacter = 0;
        private uint _currentWorld = 0;

        private bool _editMode = false;
        private string _newName = "";



        private Dictionary<Character, PopupMenu> _popupMenus = new();
        public PopupMenu GetCharacterMenu(Character character)
        {
            if (!_popupMenus.ContainsKey(character))
            {
                _popupMenus[character] = new PopupMenu("cm_" + character.CharacterId, PopupMenu.PopupMenuButtons.Right,
                    new List<PopupMenu.IPopupMenuItem>()
                    {
                        new PopupMenu.PopupMenuItemSelectableConfirm("清除背包", "ci_" + character.CharacterId, "确定要清除该 " + character.CharacterType.FormattedName() + "?", ClearInventories, "清除该 " + character.CharacterType.FormattedName() + "?"),
                        new PopupMenu.PopupMenuItemSelectableConfirm("删除 " + character.CharacterType.FormattedName(), "dc_" + character.CharacterId, "确定要删除该 " + character.CharacterType.FormattedName() + "?", DeleteCharacter, "删除该 " + character.CharacterType.FormattedName() + "?"),
                    }
                );
            }

            return _popupMenus[character];
        }

        private void DeleteCharacter(string arg1, bool arg2)
        {
            if (arg2)
            {
                var characterId = ulong.Parse(arg1.Split("dc_", StringSplitOptions.RemoveEmptyEntries)[0]);
                _characterMonitor.RemoveCharacter(characterId);
                _inventoryMonitor.ClearCharacterInventories(characterId);
            }
        }

        private void ClearInventories(string arg1, bool arg2)
        {
            if (arg2)
            {
                var characterId = ulong.Parse(arg1.Split("ci_", StringSplitOptions.RemoveEmptyEntries)[0]);
                _inventoryMonitor.ClearCharacterInventories(characterId);
            }
        }

        public override List<MessageBase>? Draw()
        {
            var messages = new List<MessageBase>();
            using (var sidebar = ImRaii.Child("charactersBar", new Vector2(160, 0) * ImGui.GetIO().FontGlobalScale, true))
            {
                if (sidebar.Success)
                {
                    var worldIds = _characterMonitor.GetWorldIds();
                    var characters = _characterMonitor.GetPlayerCharacters().Where(c => _currentWorld == 0 || _currentWorld == c.Value.WorldId).OrderBy(c => c.Value.FormattedName).ToList();
                    ImGui.TextUnformatted("角色（" + characters.Count + ")");
                    ImGui.Separator();
                    for (var index = 0; index < characters.Count; index++)
                    {
                        var character = characters[index];
                        using (ImRaii.PushId(index))
                        {
                            if (ImGui.Selectable(character.Value.FormattedName))
                            {
                                _selectedCharacter = character.Key;
                            }

                            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                            {
                                ImGui.OpenPopup("cm_" + character.Key);
                            }

                            GetCharacterMenu(character.Value).Draw();

                            var tooltip = character.Value.FormattedName;
                            if (character.Value.ActualClassJob != null)
                            {
                                tooltip += "\n" + character.Value.ActualClassJob?.Base.Name.ExtractText().ToTitleCase();
                            }

                            tooltip += "\n\n右键：选项";
                            ImGuiUtil.HoverTooltip(tooltip);
                            ImGui.SameLine();
                            if (character.Value.ActualClassJob != null)
                            {
                                var icon = ImGuiService.GetIconTexture(character.Value.Icon);
                                ImGui.Image(icon.Handle, new Vector2(16, 16) * ImGui.GetIO().FontGlobalScale);
                            }
                        }
                    }
                    ImGui.NewLine();

                    var freeCompanies = _characterMonitor.GetFreeCompanies().Where(c => _currentWorld == 0 || _currentWorld == c.Value.WorldId).OrderBy(c => c.Value.FormattedName).ToList();
                    ImGui.TextUnformatted("部队（" + freeCompanies.Count + ")");
                    ImGui.Separator();
                    for (var index = 0; index < freeCompanies.Count; index++)
                    {
                        var freeCompany = freeCompanies[index];
                        if (ImGui.Selectable(freeCompany.Value.FormattedName))
                        {
                            _selectedCharacter = freeCompany.Key;
                        }

                        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        {
                            ImGui.OpenPopup("cm_" + freeCompany.Key);
                        }

                        GetCharacterMenu(freeCompany.Value).Draw();
                        var tooltip = freeCompany.Value.FormattedName;

                        tooltip += "\n\n右键：选项";
                        ImGuiUtil.HoverTooltip(tooltip);
                        if (freeCompany.Value.ActualClassJob != null)
                        {
                            ImGui.SameLine();
                            var icon = ImGuiService.GetIconTexture(freeCompany.Value.Icon);
                            ImGui.Image(icon.Handle, new Vector2(16,16) * ImGui.GetIO().FontGlobalScale);
                        }
                    }
                    ImGui.NewLine();

                    var houses = _characterMonitor.GetHouses().Where(c => _currentWorld == 0 || _currentWorld == c.Value.WorldId).OrderBy(c => c.Value.FormattedName).ToList();
                    ImGui.TextUnformatted("住宅（" + houses.Count + ")");
                    ImGui.Separator();
                    for (var index = 0; index < houses.Count; index++)
                    {
                        var house = houses[index];
                        if (ImGui.Selectable(house.Value.FormattedName))
                        {
                            _selectedCharacter = house.Key;
                        }
                        if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                        {
                            ImGui.OpenPopup("cm_" + house.Key);
                        }

                        GetCharacterMenu(house.Value).Draw();
                        var tooltip = house.Value.FormattedName;
                        tooltip += "\n" + house.Value.GetPlotSize().ToString();

                        tooltip += "\n\n右键：选项";
                        ImGuiUtil.HoverTooltip(tooltip);

                        if (house.Value.ActualClassJob != null)
                        {
                            ImGui.SameLine();
                            var icon = ImGuiService.GetIconTexture(house.Value.Icon);
                            ImGui.Image(icon.Handle, new Vector2(16,16) * ImGui.GetIO().FontGlobalScale);
                        }
                    }
                    ImGui.NewLine();

                    var retainers = _characterMonitor.GetRetainerCharacters().Where(c => _currentWorld == 0 || _currentWorld == c.Value.WorldId).OrderBy(c => c.Value.FormattedName).ToList();
                    ImGui.TextUnformatted("雇员（" + retainers.Count + ")");
                    ImGui.Separator();

                    for (var index = 0; index < characters.Count; index++)
                    {
                        var character = characters[index];
                        var characterRetainers = _characterMonitor.GetRetainerCharacters(character.Key).Where(c => _currentWorld == 0 || _currentWorld == c.Value.WorldId).OrderBy(c => c.Value.FormattedName).ToList();
                        ImGui.TextUnformatted(character.Value.FormattedName + " (" + characterRetainers.Count + ")");
                        ImGui.Separator();
                        for (var index2 = 0; index2 < characterRetainers.Count; index2++)
                        {
                            var characterRetainer = characterRetainers[index2];
                            retainers.Remove(characterRetainer);
                            if (ImGui.Selectable(characterRetainer.Value.FormattedName))
                            {
                                _selectedCharacter = characterRetainer.Key;
                            }
                            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                            {
                                ImGui.OpenPopup("cm_" + characterRetainer.Key);
                            }

                            GetCharacterMenu(characterRetainer.Value).Draw();
                            var tooltip = characterRetainer.Value.FormattedName;
                            if (characterRetainer.Value.ActualClassJob != null)
                            {
                                tooltip += "\n" + characterRetainer.Value.ActualClassJob?.Base.Name.ExtractText().ToTitleCase();
                            }

                            tooltip += "\n\n右键：选项";
                            ImGuiUtil.HoverTooltip(tooltip);
                            if (characterRetainer.Value.ActualClassJob != null)
                            {
                                ImGui.SameLine();
                                var icon = ImGuiService.GetIconTexture(characterRetainer.Value.Icon);
                                ImGui.Image(icon.Handle, new Vector2(16,16) * ImGui.GetIO().FontGlobalScale);
                            }
                        }
                        ImGui.NewLine();
                    }

                    if (retainers.Count != 0)
                    {
                        ImGui.TextUnformatted("孤立雇员：");
                        ImGui.Separator();
                        for (var index2 = 0; index2 < retainers.Count; index2++)
                        {
                            var characterRetainer = retainers[index2];
                            if (ImGui.Selectable(characterRetainer.Value.FormattedName))
                            {
                                _selectedCharacter = characterRetainer.Key;
                            }

                            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
                            {
                                ImGui.OpenPopup("cm_" + characterRetainer.Key);
                            }

                            GetCharacterMenu(characterRetainer.Value).Draw();
                            var tooltip = characterRetainer.Value.FormattedName;
                            if (characterRetainer.Value.ActualClassJob != null)
                            {
                                tooltip += "\n" + characterRetainer.Value.ActualClassJob?.Base.Name.ExtractText().ToTitleCase();
                            }

                            tooltip += "\n\n右键：选项";
                            ImGuiUtil.HoverTooltip(tooltip);
                            if (characterRetainer.Value.ActualClassJob != null)
                            {
                                ImGui.SameLine();
                                var icon = ImGuiService.GetIconTexture(characterRetainer.Value.Icon);
                                ImGui.Image(icon.Handle, new Vector2(16, 16) * ImGui.GetIO().FontGlobalScale);
                            }
                        }

                        ImGui.NewLine();
                    }

                    World? selectedWorld = null;
                    if (_currentWorld != 0)
                    {
                        selectedWorld = _worldSheet.GetRowOrDefault(_currentWorld);
                    }

                    ImGui.Text("服务器：");
                    using var combo = ImRaii.Combo("##activeWorld", selectedWorld?.Name.ExtractText() ?? "全部");
                    if (combo.Success)
                    {
                        if (ImGui.Selectable("全部"))
                        {
                            _currentWorld = 0;
                        }

                        var worlds = _worldSheet.Where(c => worldIds.Contains(c.RowId)).ToList();
                        foreach (var world in worlds)
                        {
                            if (ImGui.Selectable(world.Name.ExtractText()))
                            {
                                _currentWorld = world.RowId;
                                _selectedCharacter = 0;
                            }
                        }
                    }
                }
            }
            ImGui.SameLine();
            using (var main = ImRaii.Child("characterMain", new Vector2(0, 0), true))
            {
                if (main.Success)
                {
                    if (_selectedCharacter != 0)
                    {
                        var character = _characterMonitor.GetCharacterById(_selectedCharacter);
                        if (character != null)
                        {
                            ImGui.Text(character.FormattedName.ToString());

                            if (character.ActualClassJob != null)
                            {
                                ImGui.SameLine();
                                var icon = ImGuiService.GetIconTexture((uint)character.Icon);
                                ImGui.Image(icon.Handle, new Vector2(16,16) * ImGui.GetIO().FontGlobalScale);
                            }

                            ImGui.SameLine();
                            var cursorX = ImGui.GetCursorPosX();
                            if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Edit, ref cursorX))
                            {
                                _editMode = true;
                                _newName = character.AlternativeName ?? "";
                            }

                            ImGuiUtil.HoverTooltip("编辑名称，将名称留空以恢复为原始名称。");

                            if (_editMode)
                            {
                                var newName = _newName;
                                ImGui.Text("自定义名称：");
                                ImGui.SameLine();
                                if (ImGui.InputText("##customName", ref newName, 100))
                                {
                                    _newName = newName;
                                }

                                if (character.AlternativeName != null && character.AlternativeName != character.Name)
                                {
                                    ImGui.Text("原始名称：" + character.Name);
                                }

                                if (ImGui.Button("保存"))
                                {
                                    if (_newName == "" || _newName == character.Name)
                                    {
                                        character.AlternativeName = null;
                                        _characterMonitor.InvokeCharacterUpdated(character);
                                        _editMode = false;
                                    }
                                    else
                                    {
                                        character.AlternativeName = _newName;
                                        _characterMonitor.InvokeCharacterUpdated(character);
                                        _editMode = false;
                                    }
                                }
                            }


                            ImGui.Separator();
                            if (character.CharacterType is CharacterType.Character or CharacterType.Retainer )
                            {
                                ImGui.Text("等级：" + character.Level);
                                // 显示角色/雇员金币。character.Gil 只对雇员有值，角色自身需从货币栏(ItemId=1)读取
                                var gil = character.Gil;
                                if (gil == 0)
                                {
                                    var currencyItems = _inventoryMonitor.GetSpecificInventory(character.CharacterId, InventoryType.Currency);
                                    var gilItem = currencyItems.FirstOrDefault(c => c.ItemId == 1);
                                    if (gilItem != null)
                                    {
                                        gil = gilItem.Quantity;
                                    }
                                }
                                ImGui.Text("金币：" + gil.ToString("N0", System.Globalization.CultureInfo.InvariantCulture));
                                ImGui.Text("性别：" + character.Gender.FormattedName());
                                ImGui.Text("部队：" + character.FreeCompanyName);
                                ImGui.Text("服务器：" + (character.World?.Name.ExtractText() ?? "Unknown"));
                                ImGui.Text("职业：" +
                                           (character.ActualClassJob?.Base.Name.ExtractText().ToTitleCase() ?? "Unknown"));
                            }
                            else if (character.CharacterType is CharacterType.Housing)
                            {
                                ImGui.Text("服务器：" + (character.World?.Name.ExtractText() ?? "Unknown"));
                                ImGui.Text("土地大小：" + character.GetPlotSize().FormattedName());
                                ImGui.Text("位置：" + character.HousingName);
                                ImGui.Text("持有者：");
                                foreach (var ownerId in character.Owners)
                                {
                                    var owner = _characterMonitor.GetCharacterById(ownerId);
                                    var ownerName = owner?.FormattedName ?? "缺少角色";
                                    ImGui.Text(ownerName);
                                }
                            }
                            else if (character.CharacterType is CharacterType.FreeCompanyChest)
                            {
                                ImGui.Text("服务器：" + (character.World?.Name.ExtractText() ?? "Unknown"));
                                ImGui.Text("关联角色：");
                                foreach (var relatedCharacter in _characterMonitor.GetFreeCompanyCharacters(character.CharacterId))
                                {
                                    var relatedCharacterName = relatedCharacter.Value.FormattedName;
                                    ImGui.Text(relatedCharacterName);
                                }
                            }

                            ImGui.NewLine();
                            ImGui.Text("背包：");
                            ImGui.Separator();
                            var inventories =
                                _inventoryMonitor.Inventories.ContainsKey(character.CharacterId)
                                    ? _inventoryMonitor.Inventories[character.CharacterId]
                                    : null;
                            if (inventories != null)
                            {
                                var categories = inventories.GetAllInventoriesByCategory();

                                using (var tabBar = ImRaii.TabBar("categories", ImGuiTabBarFlags.FittingPolicyScroll | ImGuiTabBarFlags.ListPopupButton))
                                {
                                    if (tabBar.Success)
                                    {
                                        foreach (var category in categories)
                                        {
                                            var inventoryWidth = 5;
                                            using (var tabItem = ImRaii.TabItem(category.Key.FormattedName()))
                                            {
                                                if (tabItem.Success)
                                                {
                                                    using (var tabBar2 = ImRaii.TabBar("types", ImGuiTabBarFlags.FittingPolicyScroll | ImGuiTabBarFlags.ListPopupButton))
                                                    {
                                                        if (tabBar2.Success)
                                                        {
                                                            var itemsByType = category.Value.GroupBy(c => c.SortedContainer);
                                                            foreach (var type in itemsByType)
                                                            {
                                                                using (var typeChild = ImRaii.TabItem(type.Key.FormattedName()))
                                                                {
                                                                    if (typeChild.Success)
                                                                    {
                                                                        var chunkedItems = type.OrderBy(c => c.Slot).Chunk(inventoryWidth);
                                                                        var realSlot = 1;
                                                                        foreach (var itemChunk in chunkedItems)
                                                                        {
                                                                            for (var index = 0;
                                                                                 index < itemChunk.Length;
                                                                                 index++)
                                                                            {
                                                                                var item = itemChunk[index];
                                                                                using (ImRaii.PushId(item.Slot))
                                                                                {
                                                                                    if (ImGui.ImageButton(item.ItemId == 0
                                                                                                ? ImGuiService
                                                                                                    .GetIconTexture(62574)
                                                                                                    .Handle
                                                                                                : ImGuiService
                                                                                                    .GetIconTexture(item.Icon)
                                                                                                    .Handle,
                                                                                            new Vector2(32, 32)))
                                                                                    {

                                                                                    }

                                                                                    ImGuiUtil.HoverTooltip(item.FormattedName +
                                                                                        " - " + item.Quantity + " 位于栏位 " +
                                                                                        realSlot);
                                                                                    ImGui.SameLine();

                                                                                    var hoveredRow = -1;

                                                                                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled & ImGuiHoveredFlags.AllowWhenOverlapped & ImGuiHoveredFlags.AllowWhenBlockedByPopup & ImGuiHoveredFlags.AllowWhenBlockedByActiveItem & ImGuiHoveredFlags.AnyWindow)) {
                                                                                        hoveredRow = realSlot;
                                                                                    }

                                                                                    if (hoveredRow == realSlot && item.ItemId != 0 && ImGui.IsMouseReleased(ImGuiMouseButton.Right))
                                                                                    {
                                                                                        ImGui.OpenPopup("RightClick" + realSlot);
                                                                                    }

                                                                                    if (hoveredRow == realSlot && item.ItemId != 0 && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
                                                                                    {
                                                                                        messages.Add(new OpenUintWindowMessage(typeof(ItemWindow), item.ItemId));
                                                                                    }

                                                                                    using (var popup = ImRaii.Popup("RightClick" + realSlot))
                                                                                    {
                                                                                        using var _ = ImRaii.PushId("RightClick" + realSlot);
                                                                                        if (popup.Success)
                                                                                        {
                                                                                            ImGuiService.ImGuiMenuService.DrawRightClickPopup(item, messages);
                                                                                        }
                                                                                    }
                                                                                }

                                                                                realSlot++;
                                                                            }

                                                                            ImGui.NewLine();
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }


                            }
                            else
                            {
                                ImGui.Text("未找到背包。");
                            }

                        }
                        else
                        {
                            ImGui.Text("选择了无效角色。");
                        }
                    }
                }
            }
            return messages;
        }

        public override bool IsMenuItem => _isSeparator;
        public override bool DrawBorder => false;
    }
}