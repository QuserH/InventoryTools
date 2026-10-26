using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.Shared.Extensions;
using AllaganLib.Shared.Time;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using DalaMock.Shared.Interfaces;
using Dalamud.Game.ClientState.Keys;
using Dalamud.Interface;
using Dalamud.Interface.Colors;
using Dalamud.Interface.Utility.Raii;
using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Common.Math;
using Humanizer;
using Dalamud.Bindings.ImGui;
using InventoryTools.Compendium.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using InventoryTools.Extensions;
using InventoryTools.Logic;
using InventoryTools.Logic.ItemRenderers;
using InventoryTools.Logic.Settings;
using InventoryTools.Mediator;
using InventoryTools.Ui;
using Lumina.Data;
using InventoryTools.Localization;

namespace InventoryTools.Services;

public class ItemInfoRenderService : IDisposable
{
    private readonly SourceIconGroupingSetting _sourceIconGroupingSetting;
    private readonly UseIconGroupingSetting _useIconGroupingSetting;
    private readonly ImGuiService _imGuiService;
    private readonly IPluginLog _pluginLog;
    private readonly InventoryToolsConfiguration _configuration;
    private readonly IClipboardService _clipboardService;
    private readonly ImGuiTooltipService _tooltipService;
    private readonly IFont _font;
    private readonly IKeyState _keyState;
    private readonly IFramework _framework;
    private readonly ICompendiumTypeFactory _compendiumTypeFactory;
    private readonly Dictionary<Type,IItemInfoRenderer> _sourceRenderers;
    private readonly Dictionary<Type,IItemInfoRenderer> _useRenderers;
    private readonly Dictionary<ItemInfoType,IItemInfoRenderer> _sourceRenderersByItemInfoType;
    private readonly Dictionary<ItemInfoType,IItemInfoRenderer> _useRenderersByItemInfoType;

    public ItemInfoRenderService(IEnumerable<IItemInfoRenderer> itemRenderers,
        SourceIconGroupingSetting sourceIconGroupingSetting, UseIconGroupingSetting useIconGroupingSetting,
        ImGuiService imGuiService, IPluginLog pluginLog, InventoryToolsConfiguration configuration,
        IClipboardService clipboardService, ImGuiTooltipService tooltipService, IFont font, IKeyState keyState, IFramework framework,
        ICompendiumTypeFactory compendiumTypeFactory)
    {
        _sourceIconGroupingSetting = sourceIconGroupingSetting;
        _useIconGroupingSetting = useIconGroupingSetting;
        _imGuiService = imGuiService;
        _pluginLog = pluginLog;
        _configuration = configuration;
        _clipboardService = clipboardService;
        _tooltipService = tooltipService;
        _font = font;
        _keyState = keyState;
        _framework = framework;
        _compendiumTypeFactory = compendiumTypeFactory;
        var itemInfoRenderers = itemRenderers.ToList();
        _sourceRenderers = itemInfoRenderers.Where(c => c.RendererType == RendererType.Source).ToDictionary(c => c.ItemSourceType, c => c);
        _useRenderers = itemInfoRenderers.Where(c => c.RendererType == RendererType.Use).ToDictionary(c => c.ItemSourceType, c => c);
        _sourceRenderersByItemInfoType = itemInfoRenderers.Where(c => c.RendererType == RendererType.Source).ToDictionary(c => c.Type, c => c);
        _useRenderersByItemInfoType = itemInfoRenderers.Where(c => c.RendererType == RendererType.Use).ToDictionary(c => c.Type, c => c);

        _framework.Update += CheckKeys;

        #if DEBUG
        foreach (var itemType in Enum.GetValues<ItemInfoType>())
        {
            if (!_sourceRenderersByItemInfoType.ContainsKey(itemType) && !_useRenderersByItemInfoType.ContainsKey(itemType))
            {
                _pluginLog.Verbose($"Missing type {itemType}");
            }
        }
        #endif
    }

    private bool _scrollLeft;
    private bool _scrollRight;

    private DateTime? _lastTooltipTime;
    private static readonly TimeSpan TooltipTimeout = TimeSpan.FromSeconds(2);

    private bool InTooltip(DateTime now)
    {
        if (_lastTooltipTime == null)
        {
            return false;
        }
        return now - _lastTooltipTime < TooltipTimeout;
    }

    private void CheckKeys(IFramework framework)
    {
        if (InTooltip(framework.LastUpdate))
        {
            if (_keyState[VirtualKey.LEFT])
            {
                _keyState[VirtualKey.LEFT] = false;
                _scrollLeft = true;
            }
            if (_keyState[VirtualKey.RIGHT])
            {
                _keyState[VirtualKey.RIGHT] = false;
                _scrollRight = true;
            }

            if (ImGui.GetIO().KeyCtrl)
            {
                var wheel = (int)ImGui.GetIO().MouseWheel;
                if (wheel != 0)
                {
                    if (wheel > 0)
                    {
                        _scrollLeft = true;
                    }
                    else
                    {
                        _scrollRight = true;
                    }
                }
            }
        }
    }

    public Dictionary<Type,IItemInfoRenderer> SourceRenderers => _sourceRenderers;
    public Dictionary<Type,IItemInfoRenderer> UseRenderers => _useRenderers;

    public string GetCategoryName(ItemInfoRenderCategory renderCategory)
    {
        switch (renderCategory)
        {
            case ItemInfoRenderCategory.Gathering:
                return "采集";
            case ItemInfoRenderCategory.Mining:
                return "采矿";
            case ItemInfoRenderCategory.Botany:
                return "园艺";
            case ItemInfoRenderCategory.EphemeralGathering:
                return "采集（临时）";
            case ItemInfoRenderCategory.TimedGathering:
                return "采集（定时）";
            case ItemInfoRenderCategory.HiddenGathering:
                return "采集（隐藏）";
            case ItemInfoRenderCategory.Fishing:
                return "钓鱼";
            case ItemInfoRenderCategory.Venture:
                return "探险委托";
            case ItemInfoRenderCategory.ExplorationVenture:
                return "委托（探索）";
            case ItemInfoRenderCategory.Crafting:
                return "制作";
            case ItemInfoRenderCategory.Leve:
                return "理符";
            case ItemInfoRenderCategory.Duty:
                return "副本";
            case ItemInfoRenderCategory.Shop:
                return "商店";
            case ItemInfoRenderCategory.House:
                return "房屋";
            case ItemInfoRenderCategory.RelicWeapon:
                return "特殊武器（战斗）";
            case ItemInfoRenderCategory.RelicTool:
                return "特殊武器（生产）";
            case ItemInfoRenderCategory.FieldOperation:
                return "特殊场景探索";
            case ItemInfoRenderCategory.OccultCrescent:
                return "秘隐新月";
            case ItemInfoRenderCategory.Pagos:
                return "恒冰之地";
            case ItemInfoRenderCategory.Pyros:
                return "涌火之地";
            case ItemInfoRenderCategory.Hydatos:
                return "丰水之地";
            case ItemInfoRenderCategory.DeepDungeon:
                return "深层迷宫";
        }

        return LocalizationService.Ui(renderCategory.ToString().Titleize());
    }

    public string GetRelationshipName(RelationshipType relationshipType)
    {
        switch (relationshipType)
        {
            case RelationshipType.None:
                return "无";
            case RelationshipType.CraftedInto:
                return "制作成";
            case RelationshipType.CraftedFrom:
                return "由以下制作";
            case RelationshipType.Rewards:
                return "奖励";
            case RelationshipType.Required:
                return "用于";
            case RelationshipType.DropsFrom:
                return "掉落自";
            case RelationshipType.Purchaseable:
                return "可购买";
            case RelationshipType.RelatedTo:
                return "关联";
            case RelationshipType.PurchasedWith:
                return "以以下购买";
            case RelationshipType.UsedIn:
                return "用于";
            case RelationshipType.StoredIn:
                return "存放于";
            case RelationshipType.CollectedFrom:
                return "采集自";
            case RelationshipType.InSet:
                return "属于套装";
        }

        return relationshipType.Humanize();
    }

    public bool HasSourceRenderer(ItemInfoType itemInfoType)
    {
        return _sourceRenderersByItemInfoType.ContainsKey(itemInfoType);
    }
    public bool HasUseRenderer(ItemInfoType itemInfoType)
    {
        return _useRenderersByItemInfoType.ContainsKey(itemInfoType);
    }

    public bool InSourceCategory(ItemInfoType itemInfoType, ItemInfoRenderCategory renderCategory)
    {
        if (_sourceRenderersByItemInfoType.TryGetValue(itemInfoType, out var value))
        {
            if (value.Categories?.Contains(renderCategory) ?? false)
            {
                return true;
            }
        }

        return false;
    }

    public bool InUseCategory(ItemInfoType itemInfoType, ItemInfoRenderCategory renderCategory)
    {
        if (_useRenderersByItemInfoType.TryGetValue(itemInfoType, out var value))
        {
            if (value.Categories?.Contains(renderCategory) ?? false)
            {
                return true;
            }
        }

        return false;
    }

    public List<IItemInfoRenderer> GetSourcesByCategory(ItemInfoRenderCategory renderCategory)
    {
        return _sourceRenderers.Where(c => c.Value.Categories?.Contains(renderCategory) ?? false).Select(c => c.Value).ToList();
    }

    public List<IItemInfoRenderer> GetUsesByCategory(ItemInfoRenderCategory renderCategory)
    {
        return _useRenderers.Where(c => c.Value.Categories?.Contains(renderCategory) ?? false).Select(c => c.Value).ToList();
    }

    public (string Singular, string? Plural) GetSourceTypeName(ItemInfoType type)
    {
        if (_sourceRenderersByItemInfoType.TryGetValue(type, out var renderer))
        {
            return (renderer.SingularName, renderer.PluralName);
        }

        return (type.ToString(), null);
    }

    public string GetSourceHelpText(ItemInfoType type)
    {
        if (_sourceRenderersByItemInfoType.TryGetValue(type, out var renderer))
        {
            return renderer.HelpText;
        }

        return "该物品可通过 " + type.ToString();
    }

    public (string Singular, string? Plural) GetSourceTypeName(Type type)
    {
        if (_sourceRenderers.TryGetValue(type, out var renderer))
        {
            return (renderer.SingularName, renderer.PluralName);
        }

        return (type.ToString(), null);
    }

    public (string Singular, string? Plural) GetUseTypeName(ItemInfoType type)
    {
        if (_useRenderersByItemInfoType.TryGetValue(type, out var renderer))
        {
            return (renderer.SingularName, renderer.PluralName);
        }

        return (type.ToString(), null);
    }

    public string GetUseHelpText(ItemInfoType type)
    {
        if (_useRenderersByItemInfoType.TryGetValue(type, out var renderer))
        {
            return renderer.HelpText;
        }

        return "该物品可用于 " + type.ToString();
    }


    public (string Singular, string? Plural) GetUseTypeName(Type type)
    {
        if (_useRenderers.TryGetValue(type, out var renderer))
        {
            return (renderer.SingularName, renderer.PluralName);
        }

        return (type.ToString(), null);
    }

    public List<List<ItemSource>> GetGroupedSources(List<ItemSource> allItemSources)
    {
        return GroupItemSources(_sourceRenderers, allItemSources);
    }

    public List<MessageBase> DrawSource(string id, List<ItemSource> itemSources, Vector2 iconSize)
    {
        return DrawItemSource(RendererType.Source, id, itemSources, iconSize);
    }

    public string GetSourceName(ItemSource itemSource)
    {
        var sourceRenderer = this._sourceRenderers.ContainsKey(itemSource.GetType()) ? this._sourceRenderers[itemSource.GetType()] : null;
        return sourceRenderer?.GetName(itemSource) ?? itemSource.Item.NameString;
    }

    public (Type, uint)? GetRelatedSourceType(ItemSource itemSource)
    {
        var sourceRenderer = this._sourceRenderers.ContainsKey(itemSource.GetType()) ? this._sourceRenderers[itemSource.GetType()] : null;
        return sourceRenderer?.RelatedType?.Invoke(itemSource);
    }

    public List<(Type, uint)>? GetRelatedSourceTypes(ItemSource itemSource)
    {
        var sourceRenderer = this._sourceRenderers.ContainsKey(itemSource.GetType()) ? this._sourceRenderers[itemSource.GetType()] : null;
        return sourceRenderer?.RelatedTypes?.Invoke(itemSource);
    }

    public (Type, uint)? GetRelatedUseType(ItemSource itemSource)
    {
        var useRenderer = this._useRenderers.ContainsKey(itemSource.GetType()) ? this._useRenderers[itemSource.GetType()] : null;
        return useRenderer?.RelatedType?.Invoke(itemSource);
    }

    public List<(Type, uint)>? GetRelatedUseTypes(ItemSource itemSource)
    {
        var useRenderer = this._useRenderers.ContainsKey(itemSource.GetType()) ? this._useRenderers[itemSource.GetType()] : null;
        return useRenderer?.RelatedTypes?.Invoke(itemSource);
    }

    public string GetSourceDescription(ItemSource itemSource)
    {
        var sourceRenderer = this._sourceRenderers.ContainsKey(itemSource.GetType()) ? this._sourceRenderers[itemSource.GetType()] : null;
        return sourceRenderer?.GetDescription(itemSource) ?? itemSource.Item.NameString;
    }

    public int GetSourceIcon(ItemSource itemSource)
    {
        var sourceRenderer = this._sourceRenderers.ContainsKey(itemSource.GetType()) ? this._sourceRenderers[itemSource.GetType()] : null;
        return sourceRenderer?.GetIcon(itemSource) ?? itemSource.Item.Icon;
    }

    public List<List<ItemSource>> GetGroupedUses(List<ItemSource> allItemSources)
    {
        return GroupItemSources(_useRenderers, allItemSources);
    }

    public List<MessageBase> DrawUse(string id, List<ItemSource> itemSources, Vector2 iconSize)
    {
        return DrawItemSource(RendererType.Use, id, itemSources, iconSize);
    }

    public string GetUseName(ItemSource itemSource)
    {
        var useRenderer = this._useRenderers.ContainsKey(itemSource.GetType()) ? this._useRenderers[itemSource.GetType()] : null;
        return useRenderer?.GetName(itemSource) ?? itemSource.Item.NameString;
    }

    public string GetUseDescription(ItemSource itemSource)
    {
        var useRenderer = this._useRenderers.ContainsKey(itemSource.GetType()) ? this._useRenderers[itemSource.GetType()] : null;
        return useRenderer?.GetDescription(itemSource) ?? itemSource.Item.NameString;
    }

    public int GetUseIcon(ItemSource itemSource)
    {
        var useRenderer = this._useRenderers.ContainsKey(itemSource.GetType()) ? this._useRenderers[itemSource.GetType()] : null;
        return useRenderer?.GetIcon(itemSource) ?? itemSource.Item.Icon;
    }

    private List<List<ItemSource>> GroupItemSources(Dictionary<Type,IItemInfoRenderer> renderers, List<ItemSource> allItemSources)
    {
        List<List<ItemSource>> groupedItems = new List<List<ItemSource>>();
        var groupedByType = allItemSources.GroupBy(c => c.GetType());
        foreach (var group in groupedByType)
        {
            if (renderers.TryGetValue(group.Key, out var renderer))
            {
                var sourceGroupings = _sourceIconGroupingSetting.CurrentValue(_configuration);
                var useGroupings = _useIconGroupingSetting.CurrentValue(_configuration);
                if (renderer.RendererType == RendererType.Source && (sourceGroupings?.ContainsKey(group.Key) ?? false))
                {
                    if (sourceGroupings[group.Key])
                    {
                        groupedItems.Add(group.ToList());
                    }
                    else
                    {
                        foreach (var ungroupedItem in group)
                        {
                            groupedItems.Add([ungroupedItem]);
                        }
                    }
                }
                else if (renderer.RendererType == RendererType.Use && (useGroupings?.ContainsKey(group.Key) ?? false))
                {
                    if (useGroupings[group.Key])
                    {
                        groupedItems.Add(group.ToList());
                    }
                    else
                    {
                        foreach (var ungroupedItem in group)
                        {
                            groupedItems.Add([ungroupedItem]);
                        }
                    }
                }
                else if (renderer.CustomGroup != null)
                {
                    var customGrouping = renderer.CustomGroup.Invoke(group.ToList());
                    groupedItems.AddRange(customGrouping);
                }
                else
                {
                    if (renderer.ShouldGroup)
                    {
                        groupedItems.Add(group.ToList());
                    }
                    else
                    {
                        foreach (var ungroupedItem in group)
                        {
                            groupedItems.Add([ungroupedItem]);
                        }
                    }
                }
            }
            else
            {
                //If no renderer assume we'll leave them ungrouped
                foreach (var ungroupedItem in group)
                {
                    groupedItems.Add([ungroupedItem]);
                }
            }
        }

        return groupedItems;
    }

    private int _itemTooltipIndex;

    private List<MessageBase> DrawItemSource(RendererType rendererType, string id, List<ItemSource> itemSources, Vector2 iconSize)
    {
        using var pushId = ImRaii.PushId(id);
        var messages = new List<MessageBase>();
        var firstItem = itemSources.First();
        var renderers = rendererType == RendererType.Source ? _sourceRenderers : _useRenderers;
        var icon = rendererType == RendererType.Source ? GetSourceIcon(firstItem) : GetUseIcon(firstItem);
        var sourceRenderer = renderers.ContainsKey(firstItem.GetType()) ? renderers[firstItem.GetType()] : null;

        var sourceIcon = _imGuiService.GetIconTexture(icon);

        var hasCustomClick = sourceRenderer?.OnClick != null;

        if (hasCustomClick && ImGui.ImageButton(sourceIcon.Handle,
                new Vector2(iconSize.X, iconSize.Y) * ImGui.GetIO().FontGlobalScale, new Vector2(0, 0),
                new Vector2(1, 1), 0))
        {
            if (itemSources.Count > 1)
            {
                ImGui.OpenPopup("PickItemSource");
            }
            else
            {
                var newMessages = sourceRenderer?.OnClick?.Invoke(firstItem);
                if (newMessages != null)
                {
                    messages.AddRange(newMessages);
                }
            }
        }
        else if(!hasCustomClick)
        {
            if (ImGui.ImageButton(sourceIcon.Handle,
                    new Vector2(iconSize.X, iconSize.Y) * ImGui.GetIO().FontGlobalScale, new Vector2(0, 0),
                    new Vector2(1, 1), 0))
            {
                var items = itemSources.SelectMany(c => c.RewardItems).DistinctBy(c => c.ItemId).ToList();
                var costItems = itemSources.SelectMany(c => c.CostItems).DistinctBy(c => c.ItemId).ToList();
                if (items.Count == 1 && costItems.Count == 0)
                {
                    messages.Add(new OpenUintWindowMessage(typeof(ItemWindow), items[0].ItemId));
                }
                else
                {
                    ImGui.OpenPopup("LeftClick");
                }
            }
        }

        using (var popup = ImRaii.Popup("LeftClick"))
        {
            if (popup.Success)
            {
                var items = itemSources.SelectMany(c => c.RewardItems).DistinctBy(c => c.ItemId).ToList();
                var costItems = itemSources.SelectMany(c => c.CostItems).DistinctBy(c => c.ItemId).ToList();

                if (rendererType == RendererType.Source)
                {
                    ImGui.Text("物品");
                    ImGui.Separator();
                    foreach (var item in items)
                    {
                        this._imGuiService.DrawIcon(item.ItemRow.Icon, new Vector2(16, 16));
                        if (ImGui.IsItemHovered())
                        {
                            this._tooltipService.DrawItemTooltip(new SearchResult(item));
                        }

                        ImGui.SameLine();
                        if (ImGui.Selectable(item.ItemRow.NameString))
                        {
                            messages.Add(new OpenUintWindowMessage(typeof(ItemWindow), item.ItemId));
                        }
                    }

                    if (costItems.Count > 0)
                    {
                        ImGui.NewLine();
                        ImGui.Text("相关物品：");
                        ImGui.Separator();
                        foreach (var item in costItems)
                        {
                            this._imGuiService.DrawIcon(item.ItemRow.Icon, new Vector2(16, 16));
                            if (ImGui.IsItemHovered())
                            {
                                this._tooltipService.DrawItemTooltip(new SearchResult(item));
                            }

                            ImGui.SameLine();
                            if (ImGui.Selectable(item.ItemRow.NameString))
                            {
                                messages.Add(new OpenUintWindowMessage(typeof(ItemWindow), item.ItemId));
                            }
                        }
                    }
                }
                else
                {
                    if (costItems.Count > 0)
                    {
                        ImGui.Text("物品：");
                        ImGui.Separator();
                        foreach (var item in costItems)
                        {
                            this._imGuiService.DrawIcon(item.ItemRow.Icon, new Vector2(16, 16));
                            if (ImGui.IsItemHovered())
                            {
                                this._tooltipService.DrawItemTooltip(new SearchResult(item));
                            }

                            ImGui.SameLine();
                            if (ImGui.Selectable(item.ItemRow.NameString))
                            {
                                messages.Add(new OpenUintWindowMessage(typeof(ItemWindow), item.ItemRow.RowId));
                            }
                        }
                    }

                    if (items.Count > 0)
                    {
                        ImGui.NewLine();
                        ImGui.Text("相关物品");
                        ImGui.Separator();
                        foreach (var item in items)
                        {
                            this._imGuiService.DrawIcon(item.ItemRow.Icon, new Vector2(16, 16));
                            if (ImGui.IsItemHovered())
                            {
                                this._tooltipService.DrawItemTooltip(new SearchResult(item));
                            }

                            ImGui.SameLine();
                            if (ImGui.Selectable(item.ItemRow.NameString))
                            {
                                messages.Add(new OpenUintWindowMessage(typeof(ItemWindow), item.ItemId));
                            }
                        }
                    }
                }
            }
        }

        if (hasCustomClick && itemSources.Count > 1)
        {
            using (var popup = ImRaii.Popup("PickItemSource"))
            {
                if (popup.Success)
                {
                    var typeName = (rendererType == RendererType.Source ? this.GetSourceTypeName(firstItem.GetType()) : this.GetUseTypeName(firstItem.GetType()));
                    ImGui.Text("选择" + (typeName.Plural ?? typeName.Singular));
                    ImGui.Separator();
                    for (var index = 0; index < itemSources.Count; index++)
                    {
                        var source = itemSources[index];
                        using (ImRaii.PushId(index))
                        {
                            if (ImGui.Selectable(sourceRenderer?.GetName(source) ?? "无名称"))
                            {
                                var newMessages = sourceRenderer?.OnClick?.Invoke(source);
                                if (newMessages != null)
                                {
                                    messages.AddRange(newMessages);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (!ImGui.IsAnyItemHovered())
        {
            _lastTooltipTime = null;
            _scrollLeft = false;
            _scrollRight = false;
            _itemTooltipIndex = 0;
        }

        DrawItemSourceTooltip(rendererType, itemSources);

        if (ImGui.IsItemHovered(ImGuiHoveredFlags.AllowWhenDisabled &
                                ImGuiHoveredFlags.AllowWhenOverlapped &
                                ImGuiHoveredFlags.AllowWhenBlockedByPopup &
                                ImGuiHoveredFlags.AllowWhenBlockedByActiveItem &
                                ImGuiHoveredFlags.AnyWindow) &&
            ImGui.IsMouseReleased(ImGuiMouseButton.Right))
        {
            ImGui.OpenPopup("RightClick");
        }

        using (var popup = ImRaii.Popup("RightClick"))
        {
            if (popup.Success)
            {
                if (sourceRenderer?.OnRightClick != null)
                {
                    sourceRenderer?.OnRightClick.Invoke(firstItem);
                }
                else
                {
                    var items = itemSources.SelectMany(c => c.RewardItems).DistinctBy(c => c.ItemId).ToList();
                    var costItems = itemSources.SelectMany(c => c.CostItems).DistinctBy(c => c.ItemId).ToList();
                    var relatedItems = itemSources.SelectMany(c => c.RelatedItems).ToList();

                    if (items.Count == 1 && costItems.Count == 0 && relatedItems.Count == 0)
                    {
                        _imGuiService.ImGuiMenuService.DrawRightClickPopup(rendererType == RendererType.Source ? firstItem.Item : (firstItem.CostItem ?? firstItem.Item), messages);
                    }
                    else
                    {
                        if (rendererType == RendererType.Source)
                        {
                            ImGui.Text(items.Count == 1 ? "物品" : "物品");
                            ImGui.Separator();
                            foreach (var item in items)
                            {
                                this._imGuiService.DrawIcon(item.ItemRow.Icon, new Vector2(16, 16));
                                if (ImGui.IsItemHovered())
                                {
                                    this._tooltipService.DrawItemTooltip(new SearchResult(item));
                                }

                                ImGui.SameLine();
                                using var menu = ImRaii.Menu(item.ItemRow.NameString);
                                if (menu)
                                {
                                    _imGuiService.ImGuiMenuService.DrawRightClickPopup(item.ItemRow, messages);
                                }
                            }

                            if (costItems.Count > 0)
                            {
                                ImGui.NewLine();
                                ImGui.Text("相关物品");
                                ImGui.Separator();
                                foreach (var item in costItems)
                                {
                                    this._imGuiService.DrawIcon(item.ItemRow.Icon, new Vector2(16, 16));
                                    if (ImGui.IsItemHovered())
                                    {
                                        this._tooltipService.DrawItemTooltip(new SearchResult(item.ItemRow));
                                    }

                                    ImGui.SameLine();
                                    using var menu = ImRaii.Menu(item.ItemRow.NameString);
                                    if (menu)
                                    {
                                        _imGuiService.ImGuiMenuService.DrawRightClickPopup(item.ItemRow, messages);
                                    }
                                }
                            }

                            if (relatedItems.Count > 0)
                            {
                                foreach (var relatedItem in relatedItems)
                                {
                                    ImGui.NewLine();
                                    ImGui.Text(relatedItem.Key.Name);
                                    ImGui.Separator();
                                    foreach (var item in relatedItem.Value)
                                    {
                                        this._imGuiService.DrawIcon(item.ItemRow.Icon, new Vector2(16, 16));
                                        if (ImGui.IsItemHovered())
                                        {
                                            this._tooltipService.DrawItemTooltip(new SearchResult(item.ItemRow));
                                        }

                                        ImGui.SameLine();
                                        using var menu = ImRaii.Menu(item.ItemRow.NameString);
                                        if (menu)
                                        {
                                            _imGuiService.ImGuiMenuService.DrawRightClickPopup(item.ItemRow, messages);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (costItems.Count > 0)
                            {
                                using (ImRaii.PushId("costItem"))
                                {
                                    ImGui.Text("物品：");
                                    ImGui.Separator();
                                    foreach (var item in costItems)
                                    {
                                        this._imGuiService.DrawIcon(item.ItemRow.Icon, new Vector2(16, 16));
                                        if (ImGui.IsItemHovered())
                                        {
                                            this._tooltipService.DrawItemTooltip(new SearchResult(item.ItemRow));
                                        }

                                        ImGui.SameLine();
                                        using var menu = ImRaii.Menu(item.ItemRow.NameString);
                                        if (menu)
                                        {
                                            _imGuiService.ImGuiMenuService.DrawRightClickPopup(item.ItemRow, messages);
                                        }
                                    }
                                }
                            }

                            if (items.Count > 0)
                            {
                                if (costItems.Count > 0)
                                {
                                    ImGui.NewLine();
                                }

                                using (ImRaii.PushId("relatedItem"))
                                {
                                    ImGui.Text("相关物品");
                                    ImGui.Separator();
                                    foreach (var item in items)
                                    {
                                        this._imGuiService.DrawIcon(item.ItemRow.Icon, new Vector2(16, 16));
                                        if (ImGui.IsItemHovered())
                                        {
                                            this._tooltipService.DrawItemTooltip(new SearchResult(item));
                                        }

                                        ImGui.SameLine();
                                        using var menu = ImRaii.Menu(item.ItemRow.NameString);
                                        if (menu)
                                        {
                                            _imGuiService.ImGuiMenuService.DrawRightClickPopup(item.ItemRow, messages);
                                        }
                                    }
                                }
                            }

                            if (relatedItems.Count > 0)
                            {
                                if (items.Count > 0)
                                {
                                    ImGui.NewLine();
                                }

                                foreach (var relatedItem in relatedItems)
                                {
                                    using (ImRaii.PushId(relatedItem.Key.Key))
                                    {
                                        ImGui.Text(relatedItem.Key.Name);
                                        ImGui.Separator();
                                        foreach (var item in relatedItem.Value)
                                        {
                                            this._imGuiService.DrawIcon(item.ItemRow.Icon, new Vector2(16, 16));
                                            if (ImGui.IsItemHovered())
                                            {
                                                this._tooltipService.DrawItemTooltip(new SearchResult(item));
                                            }

                                            ImGui.SameLine();
                                            using var menu = ImRaii.Menu(item.ItemRow.NameString);
                                            if (menu)
                                            {
                                                _imGuiService.ImGuiMenuService.DrawRightClickPopup(item.ItemRow,
                                                    messages);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (sourceRenderer != null)
                {
                    ImGui.Separator();
                    if (ImGui.Selectable(rendererType == RendererType.Source
                            ? "复制来源信息"
                            : "复制用途信息"))
                    {
                        var typeName = rendererType == RendererType.Source
                            ? this.GetSourceTypeName(firstItem.GetType())
                            : this.GetUseTypeName(firstItem.GetType());
                        var typeDescriptions =
                            String.Join(", ", itemSources.Select(c => sourceRenderer.GetDescription(c)));
                        var clipboardText =
                            $"{firstItem.Item.NameString}:\n\n{(itemSources.Count > 1 ? (typeName.Plural ?? typeName.Singular) : typeName.Singular)}: {typeDescriptions}";
                        _clipboardService.CopyToClipboard(clipboardText);
                    }
                }

                var relatedTypes = itemSources.Select(c => rendererType == RendererType.Source ? (c, this.GetRelatedSourceType(c)) : (c, this.GetRelatedUseType(c))).Where(c => c.Item2 != null).GroupBy(c => c.Item2.Value.Item1).ToList();
                if (relatedTypes.Count > 0)
                {
                    ImGui.NewLine();
                    ImGui.Text("图鉴条目");
                    ImGui.Separator();

                    foreach (var relatedGroup in relatedTypes)
                    {
                        var compendiumType = _compendiumTypeFactory.GetByType(relatedGroup.Key);
                        if (compendiumType != null)
                        {
                            var compendiumIcon = compendiumType.Icon;
                            this._imGuiService.DrawIcon(compendiumIcon, new Vector2(16, 16));
                            ImGui.SameLine();
                            using (var menu = ImRaii.Menu(relatedGroup.Count() == 1 ? compendiumType.Singular : compendiumType.Plural))
                            {
                                if (menu)
                                {
                                    foreach (var relatedType in relatedGroup)
                                    {
                                        var rowId = relatedType.Item2!.Value.Item2;
                                        var newId = compendiumType.RemapType(relatedType.Item2!.Value.Item1, rowId);
                                        if (newId != null)
                                        {
                                            rowId = newId.Value;
                                        }
                                        if (compendiumType.HasRow(rowId))
                                        {
                                            var name = compendiumType.GetName(rowId);
                                            if (ImGui.MenuItem(name))
                                            {
                                                messages.Add(new OpenCompendiumViewMessage(compendiumType, rowId));
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

        return messages;
    }

    public void DrawItemSourceTooltip(RendererType rendererType, List<ItemSource> itemSources)
    {
        if (itemSources.Count == 0)
        {
            return;
        }
        var firstItem = itemSources[0];
        var renderers = rendererType == RendererType.Source ? _sourceRenderers : _useRenderers;
        var sourceRenderer = renderers.ContainsKey(firstItem.GetType()) ? renderers[firstItem.GetType()] : null;
        var hasTooltip = sourceRenderer?.DrawTooltip != null;
        var hasGroupedTooltip = sourceRenderer?.DrawTooltipGrouped != null;

        if ((hasTooltip || hasGroupedTooltip) && ImGui.IsItemHovered())
        {
            _lastTooltipTime = _framework.LastUpdate;
            ImGui.SetNextWindowSizeConstraints( new System.Numerics.Vector2(250, -1), new System.Numerics.Vector2(1000,1000));
            using (ImRaii.Tooltip())
            {
                if (itemSources.Count > 1)
                {
                    var typeName = (rendererType == RendererType.Source ? this.GetSourceTypeName(firstItem.GetType()) : this.GetUseTypeName(firstItem.GetType()));
                    ImGui.Text(typeName.Plural ?? typeName.Singular);
                    ImGui.Separator();
                    if (hasGroupedTooltip)
                    {
                        sourceRenderer?.DrawTooltipGrouped?.Invoke(itemSources);
                    }
                    else
                    {

                        int totalSources = itemSources.Count;

                        if (totalSources > 0)
                        {
                            var currentIndex = _itemTooltipIndex;

                            if (currentIndex < 0 || currentIndex >= itemSources.Count)
                            {
                                currentIndex = 0;
                            }

                            var source = itemSources[currentIndex];
                            sourceRenderer?.DrawTooltip.Invoke(source);

                            ImGui.Spacing();
                            ImGui.Separator();
                            ImGui.Spacing();

                            float windowWidth = ImGui.GetContentRegionAvail().X;
                            string leftText = FontAwesomeIcon.ArrowLeft.ToIconString();
                            string centerText = $"Source {currentIndex + 1} of {totalSources}";
                            string rightText =  FontAwesomeIcon.ArrowRight.ToIconString();
                            string keyboardIcon = FontAwesomeIcon.Keyboard.ToIconString();

                            float centerWidth = ImGui.CalcTextSize(centerText).X;

                            var windowPos = ImGui.GetCursorScreenPos();

                            float leftWidth;
                            float rightWidth;

                            using (ImRaii.PushFont(_font.IconFont))
                            {
                                leftWidth = ImGui.CalcTextSize(leftText).X + ImGui.CalcTextSize(keyboardIcon).X + ImGui.GetStyle().ItemSpacing.X;
                                rightWidth = ImGui.CalcTextSize(rightText).X + ImGui.CalcTextSize(keyboardIcon).X + ImGui.GetStyle().ItemSpacing.X;
                            }

                            float availableWidth = windowWidth - leftWidth - rightWidth;
                            float centerStartPos = leftWidth + (availableWidth - centerWidth) * 0.5f;

                            ImGui.SetCursorPosX(centerStartPos);
                            ImGui.TextUnformatted(centerText);

                            var drawList = ImGui.GetWindowDrawList();
                            var textColor = ImGui.GetColorU32(ImGuiCol.Text);

                            using (ImRaii.PushFont(_font.IconFont))
                            {


                                var leftArrowPos = new System.Numerics.Vector2(windowPos.X, windowPos.Y);
                                drawList.AddText(_font.IconFont, ImGui.GetFontSize(), leftArrowPos, textColor, leftText);

                                var leftIconPos = new System.Numerics.Vector2(windowPos.X+ ImGui.CalcTextSize(keyboardIcon).X + ImGui.GetStyle().ItemSpacing.X, windowPos.Y);
                                drawList.AddText(_font.IconFont, ImGui.GetFontSize(), leftIconPos, textColor, keyboardIcon);

                                var rightIconPos = new System.Numerics.Vector2(windowPos.X + windowWidth - rightWidth, windowPos.Y);
                                drawList.AddText(_font.IconFont, ImGui.GetFontSize(), rightIconPos, textColor, keyboardIcon);

                                var rightArrowPos = new System.Numerics.Vector2(windowPos.X + windowWidth - rightWidth + ImGui.CalcTextSize(keyboardIcon).X + ImGui.GetStyle().ItemSpacing.X, windowPos.Y);
                                drawList.AddText(_font.IconFont, ImGui.GetFontSize(), rightArrowPos, textColor, rightText);
                            }

                            if (_scrollLeft)
                            {
                                currentIndex = (currentIndex - 1 + totalSources) % totalSources;
                                _itemTooltipIndex = currentIndex;
                                _scrollLeft = false;
                            }
                            if (_scrollRight)
                            {
                                currentIndex = (currentIndex + 1) % totalSources;
                                _itemTooltipIndex = currentIndex;
                                _scrollRight = false;
                            }
                        }
                    }
                }
                else
                {
                    ImGui.Text((rendererType == RendererType.Source
                        ? this.GetSourceTypeName(firstItem.GetType())
                        : this.GetUseTypeName(firstItem.GetType())).Singular);
                    ImGui.Separator();
                    sourceRenderer?.DrawTooltip.Invoke(firstItem);
                }
            }
        }
        else if(ImGui.IsItemHovered())
        {
            _lastTooltipTime = _framework.LastUpdate;
            using (ImRaii.Tooltip())
            {
                ImGui.Text("未配置悬浮提示：" + (rendererType == RendererType.Source
                    ? this.GetSourceTypeName(firstItem.GetType())
                    : this.GetUseTypeName(firstItem.GetType())).Singular + "，请报告此问题！");
            }
        }
    }

    public List<MessageBase> DrawItemSourceIconsContainer(string id, float rowSize, Vector2 iconSize, List<ItemSource> itemSources)
    {
        var messages = new List<MessageBase>();
        using var pushId = ImRaii.PushId(id);
        var count = 0;
        var groupedSources = GetGroupedSources(itemSources);
        _imGuiService.WrapTableColumnElements(id, groupedSources,
            rowSize,
            itemList =>
            {
                messages.AddRange(this.DrawSource(count.ToString(), itemList, iconSize));
                count++;
                return true;
            });

        return messages;
    }

    public List<MessageBase> DrawItemSourceIcons(string id, Vector2 iconSize, List<ItemSource> itemSources)
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        float windowVisibleX2 = ImGui.GetWindowPos().X + ImGui.GetWindowContentRegionMax().X;

        var messages = new List<MessageBase>();
        using var pushId = ImRaii.PushId(id);
        var groupedSources = GetGroupedSources(itemSources);

        for (var index = 0; index < groupedSources.Count; index++)
        {
            var groupedSource = groupedSources[index];
            messages.AddRange(DrawSource(index.ToString(), groupedSource, iconSize));

            float lastButtonX2 = ImGui.GetItemRectMax().X;
            float nextButtonX2 = lastButtonX2 + style.ItemSpacing.X + 32;

            if (index + 1 < groupedSources.Count && nextButtonX2 < windowVisibleX2)
            {
                ImGui.SameLine();
            }
        }

        return messages;
    }

    public List<MessageBase> DrawItemUseIcons(string id, Vector2 iconSize, List<ItemSource> itemSources)
    {
        ImGuiStylePtr style = ImGui.GetStyle();
        float windowVisibleX2 = ImGui.GetWindowPos().X + ImGui.GetWindowContentRegionMax().X;

        var messages = new List<MessageBase>();
        using var pushId = ImRaii.PushId(id);
        var groupedSources = GetGroupedUses(itemSources);

        for (var index = 0; index < groupedSources.Count; index++)
        {
            var groupedSource = groupedSources[index];
            messages.AddRange(DrawUse(index.ToString(), groupedSource, iconSize));

            float lastButtonX2 = ImGui.GetItemRectMax().X;
            float nextButtonX2 = lastButtonX2 + style.ItemSpacing.X + 32;

            if (index + 1 < groupedSources.Count && nextButtonX2 < windowVisibleX2)
            {
                ImGui.SameLine();
            }
        }

        return messages;
    }

    public List<MessageBase> DrawItemUseIconsContainer(string id, float rowSize, Vector2 iconSize, List<ItemSource> itemSources)
    {
        var messages = new List<MessageBase>();
        using var pushId = ImRaii.PushId(id);
        var count = 0;
        var groupedSources = GetGroupedUses(itemSources);
        _imGuiService.WrapTableColumnElements(id, groupedSources,
            rowSize,
            itemList =>
            {
                messages.AddRange(this.DrawUse(count.ToString(), itemList, iconSize));
                count++;
                return true;
            });

        return messages;
    }

    public void Dispose()
    {
        _framework.Update -= CheckKeys;
    }
}