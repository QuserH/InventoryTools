using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib.Extensions;
using CriticalCommonLib.Services;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface.Colors;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using InventoryTools.Localizers;
using InventoryTools.Logic;
using InventoryTools.Logic.Editors;
using InventoryTools.Logic.Settings;
using OtterGui.Extensions;
using OtterGui.Raii;
using InventoryTools.Localization;

namespace InventoryTools.Services;

public class ImGuiTooltipService
{
    private readonly InventoryToolsConfiguration _configuration;
    private readonly IInventoryMonitor _inventoryMonitor;
    private readonly ICharacterMonitor _characterMonitor;
    private readonly InventoryScopeCalculator _inventoryScopeCalculator;
    private readonly IKeyState _keyState;
    private readonly ITextureProvider _textureProvider;
    private readonly ItemLocalizer _itemLocalizer;
    private readonly TryOn _tryOn;
    private readonly IChatUtilities _chatUtilities;

    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public ItemInfoRenderService InfoRenderService { get; set; }

    public ImGuiTooltipService(
        InventoryToolsConfiguration configuration,
        IInventoryMonitor inventoryMonitor,
        ICharacterMonitor characterMonitor,
        InventoryScopeCalculator inventoryScopeCalculator,
        IKeyState keyState,
        ITextureProvider textureProvider,
        ItemLocalizer itemLocalizer,
        TryOn tryOn,
        IChatUtilities chatUtilities)
    {
        _configuration = configuration;
        _inventoryMonitor = inventoryMonitor;
        _characterMonitor = characterMonitor;
        _inventoryScopeCalculator = inventoryScopeCalculator;
        _keyState = keyState;
        _textureProvider = textureProvider;
        _itemLocalizer = itemLocalizer;
        _tryOn = tryOn;
        _chatUtilities = chatUtilities;
    }

    public void DrawItemTooltip(ItemInfo itemInfo)
    {
        DrawItemTooltip(itemInfo.ItemRow);
    }

    public void DrawItemTooltip(SearchResult searchResult)
    {
        DrawItemTooltip(searchResult.Item);
    }

    public void DrawItemTooltip(ItemRow item)
    {
        if (ImGui.IsItemHovered())
        {
            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                if (this._keyState[VirtualKey.CONTROL])
                {
                    this._chatUtilities.LinkItem(item);
                }
                else if (this._keyState[VirtualKey.SHIFT])
                {
                    this._tryOn.TryOnItem(item);
                }
            }

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup("rMenu");
            }
            ImGui.SetNextWindowSizeConstraints(new Vector2(200,100), new Vector2(600,600));
            using (var tooltip = ImRaii.Tooltip())
            {
                if (tooltip)
                {
                    var availableWidth = ImGui.GetContentRegionAvail().X;
                    float imageStartX = availableWidth - 32;
                    ImGui.PushTextWrapPos(imageStartX);
                    ImGui.TextUnformatted(item.NameString);
                    ImGui.PopTextWrapPos();
                    ImGui.SameLine();
                    ImGui.SetCursorPosX(ImGui.GetCursorPosX() + ImGui.GetContentRegionAvail().X - 32);
                    ImGui.Image(this._textureProvider.GetFromGameIcon(new(item.Base.Icon)).GetWrapOrEmpty().Handle, new Vector2(32, 32));
                    ImGui.TextUnformatted(item.Base.ItemUICategory.Value.Name.ExtractText());
                    ImGui.Separator();
                    if (item.ClassJobCategory != null)
                    {
                        var classJobCategory = item.ClassJobCategory.Base.Name.ExtractText();
                        if (classJobCategory != string.Empty)
                        {
                            ImGui.TextUnformatted(classJobCategory);
                        }
                    }

                    if (item.Base.BaseParamValue.All(c => c == 0))
                    {
                        DrawBaseAttributes(item);
                    }
                    else
                    {

                        using (var table = ImRaii.Table("StatsTable", 2,
                                   ImGuiTableFlags.BordersInnerV | ImGuiTableFlags.RowBg))
                        {
                            if (table)
                            {
                                ImGui.TableSetupColumn(LocalizationService.Ui("BaseAttributes"));
                                ImGui.TableSetupColumn(LocalizationService.Ui("Attributes"));
                                ImGui.TableNextRow();
                                ImGui.TableNextColumn();
                                DrawBaseAttributes(item);

                                ImGui.TableNextColumn();
                                {
                                    for (var index = 0; index < item.Base.BaseParam.Count; index++)
                                    {
                                        var baseParam = item.Base.BaseParam[index];
                                        if (baseParam.RowId == 0)
                                        {
                                            continue;
                                        }

                                        var baseParamValue = item.Base.BaseParamValue[index];
                                        if (baseParamValue == 0)
                                        {
                                            continue;
                                        }

                                        ImGui.Text(baseParam.Value.Name.ToImGuiString() + ": " +
                                                   baseParamValue);
                                    }

                                    if (item.Base.BaseParamValueSpecial.Any(c => c != 0))
                                    {
                                        ImGui.NewLine();
                                        ImGui.Separator();
                                        ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("When HQ:")));
                                        for (var index = 0; index < item.Base.BaseParamSpecial.Count; index++)
                                        {
                                            var baseParamSpecial = item.Base.BaseParamSpecial[index];
                                            if (baseParamSpecial.RowId == 0)
                                            {
                                                continue;
                                            }

                                            var baseParamValue = item.Base.BaseParamValueSpecial[index];
                                            if (baseParamValue == 0)
                                            {
                                                continue;
                                            }

                                            for (var baseParamIndex = 0; baseParamIndex < item.Base.BaseParam.Count; baseParamIndex++)
                                            {
                                                var baseParam = item.Base.BaseParam[baseParamIndex];

                                                if (baseParam.RowId == baseParamSpecial.RowId)
                                                {
                                                    baseParamValue += item.Base.BaseParamValue[baseParamIndex];
                                                }

                                            }

                                            ImGui.Text(baseParamSpecial.Value.Name.ToImGuiString() + ": " +
                                                       baseParamValue);
                                        }
                                    }

                                }
                            }
                        }
                    }
                    ImGui.Dummy(new Vector2(0, 0));
                    if (item.Sources.Count > 0)
                    {
                        ImGui.NewLine();
                        ImGui.TextUnformatted(LocalizationService.Ui(LocalizationService.Ui("Available From: ")));
                        ImGui.Separator();
                        ImGui.PushTextWrapPos();
                        var sources = item.Sources.Select(c => c.Type).Distinct().Select(
                                              c => this.InfoRenderService.GetSourceTypeName(c).Singular).Select(c => c!);
                        ImGui.TextUnformatted(string.Join(", ", sources));
                        ImGui.PopTextWrapPos();
                    }


                    if (item.Uses.Count > 0)
                    {
                        ImGui.NewLine();
                        ImGui.TextUnformatted(LocalizationService.Ui(LocalizationService.Ui("Used In: ")));
                        ImGui.Separator();
                        ImGui.PushTextWrapPos();
                        var uses = item.Uses.Select(c => c.Type).Distinct().Select(
                                              c => this.InfoRenderService.GetUseTypeName(c).Singular).Select(c => c!);
                        ImGui.TextUnformatted(string.Join(", ", uses));
                        ImGui.PopTextWrapPos();
                    }

                    var sortMode = _configuration.TooltipAmountOwnedSort;

                    var enumerable = _inventoryMonitor.AllItems.Where(inventoryItem =>
                        item.RowId == inventoryItem.ItemId &&
                        _characterMonitor.Characters.ContainsKey(inventoryItem.RetainerId) &&
                        ((_configuration.TooltipCurrentCharacter &&
                          _characterMonitor.BelongsToActiveCharacter(inventoryItem.RetainerId)) ||
                         !_configuration.TooltipCurrentCharacter)
                    );
                    if (_configuration.TooltipSearchScope != null && _configuration.TooltipSearchScope.Count != 0)
                    {
                        enumerable = enumerable.Where(c => _inventoryScopeCalculator.Filter(_configuration.TooltipSearchScope, c));
                    }

                    if (sortMode == TooltipAmountOwnedSort.Alphabetically)
                    {
                        var characterNames = _characterMonitor.Characters.OrderBy(c => c.Value.FormattedName).ToList();
                        enumerable = enumerable.OrderBy(c => characterNames.IndexOf(d => d.Key == c.RetainerId));
                    }
                    else if(sortMode == TooltipAmountOwnedSort.Categorically)
                    {
                        var characterNames = _characterMonitor.Characters.OrderBy(c => c.Value.FormattedName).ToList();
                        enumerable = enumerable.OrderBy(c => c.SortedCategory.FormattedName()).ThenBy(c => characterNames.IndexOf(d => d.Key == c.RetainerId));
                    }
                    else if(sortMode == TooltipAmountOwnedSort.Quantity)
                    {
                        enumerable = enumerable.OrderByDescending(c => c.Quantity);
                    }

                    var ownedItems = enumerable
                        .ToList();



                    uint storageCount = 0;
                    List<string> locations = new List<string>();

                    if (_configuration.TooltipLocationDisplayMode ==
                        TooltipLocationDisplayMode.CharacterBagSlotQuality)
                    {
                        foreach (var oItem in ownedItems)
                        {
                            storageCount += oItem.Quantity;

                            if (locations.Count >= _configuration.TooltipLocationLimit)
                                continue;

                            var name = _characterMonitor.GetCharacterNameById(oItem.RetainerId);
                            if (_configuration.TooltipAddCharacterNameOwned)
                            {
                                var owner = _characterMonitor.GetCharacterNameById(
                                    oItem.RetainerId, true);
                                if (owner.Trim().Length != 0)
                                    name += " (" + owner + ")";
                            }

                            var typeIcon = "";
                            if (oItem.IsHQ)
                            {
                                typeIcon = "\uE03c";
                            }
                            else if (oItem.IsCollectible)
                            {
                                typeIcon = LocalizationService.Ui("\\uE03d");
                            }

                            locations.Add($"{name} - {_itemLocalizer.FormattedBagLocation(oItem)} " + typeIcon);
                        }
                        if (ownedItems.Count > _configuration.TooltipLocationLimit)
                        {
                            locations.Add(ownedItems.Count - _configuration.TooltipLocationLimit + LocalizationService.Ui(" 个其他位置."));
                        }
                    }
                    if (_configuration.TooltipLocationDisplayMode ==
                        TooltipLocationDisplayMode.CharacterBagSlotQuantity)
                    {
                        foreach (var oItem in ownedItems)
                        {
                            storageCount += oItem.Quantity;

                            if (locations.Count >= _configuration.TooltipLocationLimit)
                                continue;

                            var name = _characterMonitor.GetCharacterNameById(oItem.RetainerId);
                            if (_configuration.TooltipAddCharacterNameOwned)
                            {
                                var owner = _characterMonitor.GetCharacterNameById(
                                    oItem.RetainerId, true);
                                if (owner.Trim().Length != 0)
                                    name += " (" + owner + ")";
                            }

                            locations.Add($"{name} - {_itemLocalizer.FormattedBagLocation(oItem)} - {+ oItem.Quantity} ");
                        }
                        if (ownedItems.Count > _configuration.TooltipLocationLimit)
                        {
                            locations.Add(ownedItems.Count - _configuration.TooltipLocationLimit + LocalizationService.Ui(" 个其他位置."));
                        }
                    }
                    else if (_configuration.TooltipLocationDisplayMode == TooltipLocationDisplayMode.CharacterCategoryQuantityQuality)
                    {
                        var groupedItems = ownedItems.GroupBy(c => (c.RetainerId, c.SortedCategory, c.Flags)).ToList();
                        foreach (var oGroup in groupedItems)
                        {
                            var quantity = oGroup.Sum(c => c.Quantity);
                            storageCount += (uint)quantity;

                            if (locations.Count >= _configuration.TooltipLocationLimit)
                                continue;

                            var name = _characterMonitor.GetCharacterNameById(oGroup.Key.RetainerId);
                            if (_configuration.TooltipAddCharacterNameOwned)
                            {
                                var owner = _characterMonitor.GetCharacterNameById(
                                    oGroup.Key.RetainerId, true);
                                if (owner.Trim().Length != 0)
                                    name += " (" + owner + ")";
                            }

                            var typeIcon = "";
                            if ((oGroup.Key.Flags & FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.HighQuality) != 0)
                            {
                                typeIcon = "\uE03c";
                            }
                            else if ((oGroup.Key.Flags & FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.Collectable) != 0)
                            {
                                typeIcon = LocalizationService.Ui("\\uE03d");
                            }

                            locations.Add($"{name} - {oGroup.Key.SortedCategory.FormattedName()} - " + quantity + " " + typeIcon);
                        }
                        if (groupedItems.Count > _configuration.TooltipLocationLimit)
                        {
                            locations.Add(groupedItems.Count - _configuration.TooltipLocationLimit + LocalizationService.Ui(" 个其他位置."));
                        }
                    }
                    else if (_configuration.TooltipLocationDisplayMode == TooltipLocationDisplayMode.CharacterWorldCategoryQuantityQuality)
                    {
                        var groupedItems = ownedItems.GroupBy(c => (c.RetainerId, c.SortedCategory, c.Flags)).ToList();
                        foreach (var oGroup in groupedItems)
                        {
                            var quantity = oGroup.Sum(c => c.Quantity);
                            storageCount += (uint)quantity;

                            if (locations.Count >= _configuration.TooltipLocationLimit)
                            {
                                continue;
                            }

                            var name = _characterMonitor.GetCharacterNameById(oGroup.Key.RetainerId);
                            if (_configuration.TooltipAddCharacterNameOwned)
                            {
                                var owner = _characterMonitor.GetCharacterNameById(oGroup.Key.RetainerId, true);
                                if (owner.Trim().Length != 0)
                                {
                                    name += " (" + owner + ")";
                                }
                            }

                            var worldName = "";
                            if (_characterMonitor.Characters.TryGetValue(oGroup.Key.RetainerId, out var character))
                            {
                                worldName = character.World?.Name.ExtractText() ?? "";
                            }

                            var typeIcon = "";
                            if ((oGroup.Key.Flags & FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.HighQuality) != 0)
                            {
                                typeIcon = "\uE03c";
                            }
                            else if ((oGroup.Key.Flags & FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.Collectable) != 0)
                            {
                                typeIcon = LocalizationService.Ui("\\uE03d");
                            }

                            var locationLine = string.IsNullOrEmpty(worldName)
                                ? $"{name} - {oGroup.Key.SortedCategory.FormattedName()} - " + quantity + " " + typeIcon
                                : $"{name} - {worldName} - {oGroup.Key.SortedCategory.FormattedName()} - " + quantity + " " + typeIcon;
                            locations.Add(locationLine);
                        }
                        if (groupedItems.Count > _configuration.TooltipLocationLimit)
                        {
                            locations.Add(groupedItems.Count - _configuration.TooltipLocationLimit + LocalizationService.Ui(" 个其他位置."));
                        }
                    }
                    else if (_configuration.TooltipLocationDisplayMode == TooltipLocationDisplayMode.CharacterQuantityQuality)
                    {
                        var groupedItems = ownedItems.GroupBy(c => (c.RetainerId, c.Flags)).ToList();
                        foreach (var oGroup in groupedItems)
                        {
                            var quantity = oGroup.Sum(c => c.Quantity);
                            storageCount += (uint)quantity;

                            if (locations.Count >= _configuration.TooltipLocationLimit)
                                continue;

                            var name = _characterMonitor.GetCharacterNameById(oGroup.Key.RetainerId);
                            if (_configuration.TooltipAddCharacterNameOwned)
                            {
                                var owner = _characterMonitor.GetCharacterNameById(
                                    oGroup.Key.RetainerId, true);
                                if (owner.Trim().Length != 0)
                                    name += " (" + owner + ")";
                            }

                            var typeIcon = "";
                            if ((oGroup.Key.Flags & FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.HighQuality) != 0)
                            {
                                typeIcon = "\uE03c";
                            }
                            else if ((oGroup.Key.Flags & FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.Collectable) != 0)
                            {
                                typeIcon = LocalizationService.Ui("\\uE03d");
                            }

                            locations.Add($"{name} - " + quantity + " " + typeIcon);
                        }
                        if (groupedItems.Count > _configuration.TooltipLocationLimit)
                        {
                            locations.Add(groupedItems.Count - _configuration.TooltipLocationLimit + LocalizationService.Ui(" 个其他位置."));
                        }
                    }
                    else if (_configuration.TooltipLocationDisplayMode == TooltipLocationDisplayMode.CharacterRetainerCategoryQuantityQuality)
                    {
                        var groupedItems = ownedItems.GroupBy(c => (c.RetainerId, c.SortedCategory, c.Flags)).ToList();
                        foreach (var oGroup in groupedItems)
                        {
                            var quantity = oGroup.Sum(c => c.Quantity);
                            storageCount += (uint)quantity;

                            if (locations.Count >= _configuration.TooltipLocationLimit)
                            {
                                continue;
                            }

                            // 判断是角色背包还是雇员背包
                            var isRetainer = oGroup.Key.RetainerId.ToString().StartsWith("3");
                            var characterName = "";
                            var retainerName = "";

                            if (isRetainer)
                            {
                                // 雇员库存：角色名 = 所属角色，雇员名 = 雇员本身
                                var retainer = _characterMonitor.GetCharacterById(oGroup.Key.RetainerId);
                                retainerName = retainer?.FormattedName ?? "";
                                characterName = _characterMonitor.GetCharacterNameById(oGroup.Key.RetainerId, true);
                            }
                            else
                            {
                                // 角色背包：角色名 = 该角色
                                var character = _characterMonitor.GetCharacterById(oGroup.Key.RetainerId);
                                characterName = character?.FormattedName ?? "";
                            }

                            var typeIcon = "";
                            if ((oGroup.Key.Flags & FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.HighQuality) != 0)
                            {
                                typeIcon = "";
                            }
                            else if ((oGroup.Key.Flags & FFXIVClientStructs.FFXIV.Client.Game.InventoryItem.ItemFlags.Collectable) != 0)
                            {
                                typeIcon = LocalizationService.Ui("\\uE03d");
                            }

                            // 角色背包：角色-分类-数量-品质；雇员背包：角色-雇员-分类-数量-品质
                            string line;
                            if (isRetainer)
                            {
                                line = $"{characterName} - {retainerName} - {oGroup.Key.SortedCategory.FormattedName()} - " + quantity + " " + typeIcon;
                            }
                            else
                            {
                                line = $"{characterName} - {oGroup.Key.SortedCategory.FormattedName()} - " + quantity + " " + typeIcon;
                            }
                            locations.Add(line);
                        }
                        if (groupedItems.Count > _configuration.TooltipLocationLimit)
                        {
                            locations.Add(groupedItems.Count - _configuration.TooltipLocationLimit + LocalizationService.Ui(" 个其他位置."));
                        }
                    }

                    if (storageCount > 0)
                    {
                        ImGui.Separator();
                        ImGui.TextUnformatted(LocalizationService.Ui("持有数量: ") + storageCount);
                        ImGui.TextUnformatted(LocalizationService.Ui("位置:"));
                        using (Dalamud.Interface.Utility.Raii.ImRaii.PushIndent())
                        {
                            for (var index = 0; index < locations.Count; index++)
                            {
                                var location = locations[index];
                                ImGui.TextUnformatted(LocalizationService.Format("{0}\n", location));
                            }
                        }
                    }

                    ImGui.Separator();
                    using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey))
                    {
                        ImGui.TextUnformatted(LocalizationService.Ui(LocalizationService.Ui("Ctrl: Link")));
                        if (item.CanTryOn)
                        {
                            ImGui.TextUnformatted(LocalizationService.Ui(LocalizationService.Ui("Shift: Try on")));
                        }
                    }
                }
            }
        }
    }

    private static void DrawBaseAttributes(ItemRow item)
    {
        ImGui.TextUnformatted(LocalizationService.Format(LocalizationService.Ui("Item Level {0}"), item.Base.LevelItem.RowId));
        if (item.ClassJobCategory != null)
        {
            ImGui.TextUnformatted(LocalizationService.Format(LocalizationService.Ui("Equip Level {0}"), item.Base.LevelEquip));
        }

        ImGui.TextUnformatted(item.FormattedRarity);

        if (item.EquipRace != CharacterRace.Any && item.EquipRace != CharacterRace.None)
        {
            ImGui.TextUnformatted(LocalizationService.Format(LocalizationService.Ui("Only equippable by {0}"), item.EquipRace.FormattedName()));
        }

        if (item.EquippableByGender != CharacterSex.Both && item.EquippableByGender != CharacterSex.NotApplicable)
        {
            ImGui.TextUnformatted(LocalizationService.Format(LocalizationService.Ui("Only equippable by {0}"), item.EquippableByGender.FormattedName()));
        }

        if (item.Base.CanBeHq)
        {
            ImGui.TextUnformatted(LocalizationService.Ui(LocalizationService.Ui("Can be HQ")));
        }

        if (item.Base.IsUnique)
        {
            ImGui.TextUnformatted(LocalizationService.Ui("Unique"));
        }

        if (item.Base.IsUntradable)
        {
            ImGui.TextUnformatted(LocalizationService.Ui("Untradable"));
        }
    }
}