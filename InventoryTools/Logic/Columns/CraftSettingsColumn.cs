using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Extensions;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Game.Text;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using DalaMock.Shared.Interfaces;
using Dalamud.Interface;
using Dalamud.Interface.Utility.Raii;
using InventoryTools.Localizers;
using InventoryTools.Logic.Columns.Abstract.ColumnSettings;
using InventoryTools.Extensions;
using InventoryTools.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Logging;
using ImGuiUtil = OtterGui.ImGuiUtil;
using InventoryItem = FFXIVClientStructs.FFXIV.Client.Game.InventoryItem;
using Vector2 = FFXIVClientStructs.FFXIV.Common.Math.Vector2;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class CraftSettingsColumn : IColumn
{
    private readonly ILogger<CraftSettingsColumn> _logger;
    private readonly CraftingCache _craftingCache;
    private readonly RecipeSheet _recipeSheet;
    private readonly MapSheet _mapSheet;
    private readonly ItemSheet _itemSheet;
    private readonly ExcelSheet<World> _worldSheet;
    private readonly CraftItemLocalizer _craftItemLocalizer;
    private readonly IngredientPreferenceLocalizer _ingredientPreferenceLocalizer;
    public ImGuiService ImGuiService { get; }
    private readonly IFont _font;

    public CraftSettingsColumn(ILogger<CraftSettingsColumn> logger, ImGuiService imGuiService,
        CraftingCache craftingCache, RecipeSheet recipeSheet, MapSheet mapSheet, ItemSheet itemSheet,
        ExcelSheet<World> worldSheet, CraftItemLocalizer craftItemLocalizer, IngredientPreferenceLocalizer ingredientPreferenceLocalizer,
        IFont font)
    {
        _logger = logger;
        _craftingCache = craftingCache;
        _recipeSheet = recipeSheet;
        _mapSheet = mapSheet;
        _itemSheet = itemSheet;
        _worldSheet = worldSheet;
        _craftItemLocalizer = craftItemLocalizer;
        _ingredientPreferenceLocalizer = ingredientPreferenceLocalizer;
        ImGuiService = imGuiService;
        _font = font;
    }
    public ColumnCategory ColumnCategory => ColumnCategory.Crafting;


    public string Name { get; set; } = "设置";
    public float Width { get; set; } = 120;
    public string HelpText { get; set; } = "在此列修改每个物品的制作设置";
    public string FilterText { get; set; } = "";

    public string RenderName { get; } = "设置";
    public virtual bool HideHeaderLabel => true;
    public List<string>? FilterChoices { get; set; } = null;
    public bool HasFilter { get; set; } = false;
    public ColumnFilterType FilterType { get; set; } = ColumnFilterType.None;
    public bool IsDebug { get; set; } = false;
    public FilterType AvailableIn { get; } = Logic.FilterType.CraftFilter;
    public virtual bool IsConfigurable => false;
    public bool IsDefault => true;

    public bool AvailableInType(FilterType type)
    {
        return type == Logic.FilterType.CraftFilter;
    }

    public bool? CraftOnly { get; } = true;

    public List<IColumnSetting> FilterSettings { get; set; } = new();
    public List<IColumnSetting> Settings { get; set; } = new();

    public string? FilterIcon { get; set; } = null;

    public IEnumerable<SearchResult> Filter(ColumnConfiguration columnConfiguration, IEnumerable<SearchResult> items)
    {
        return items;
    }

    public IEnumerable<SearchResult> Sort(ColumnConfiguration columnConfiguration, ImGuiSortDirection direction, IEnumerable<SearchResult> items)
    {
        return items;
    }

    public List<MessageBase>? Draw(FilterConfiguration configuration, ColumnConfiguration columnConfiguration,
        SearchResult searchResult,
        int rowIndex, int columnIndex)
    {
        if (searchResult.CraftItem == null) return null;

        ImGui.TableNextColumn();
        if (!ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled)) return null;

        using var id = ImRaii.PushId(rowIndex);

        using (var popup = ImRaii.Popup("ConfigureItemSettings" + columnIndex + searchResult.CraftItem.ItemId + (searchResult.CraftItem.IsOutputItem ? "o" : "")))
        {
            if (popup.Success)
            {
                ImGui.Text("配置获取方式：");
                ImGui.Separator();

                DrawRecipeSelector(configuration, searchResult.CraftItem, rowIndex);
                DrawHqSelector(configuration, searchResult.CraftItem, rowIndex);
                DrawRetainerRetrievalSelector(configuration, searchResult.CraftItem, rowIndex);
                DrawSourceSelector(configuration, searchResult.CraftItem, rowIndex);
                DrawZoneSelector(configuration, searchResult.CraftItem, rowIndex);
                DrawMarketWorldSelector(configuration, searchResult.CraftItem, rowIndex);
                DrawMarketPriceSelector(configuration, searchResult.CraftItem, rowIndex);
            }
        }

        using (var popup = ImRaii.Popup("ConfigureRecipeSettings" + rowIndex))
        {
            if (popup.Success)
            {
                ImGui.Text("配置配方：");
                ImGui.Separator();
                if (DrawRecipeSelector(configuration, searchResult.CraftItem, rowIndex))
                {
                    ImGui.CloseCurrentPopup();
                }
            }
        }

        using (var popup = ImRaii.Popup("ConfigureHQSettings" + rowIndex))
        {
            if (popup.Success)
            {
                ImGui.Text("设置需求HQ：");
                ImGui.Separator();
                if (DrawHqSelector(configuration, searchResult.CraftItem, rowIndex))
                {
                    ImGui.CloseCurrentPopup();
                }
            }
        }

        using (var popup = ImRaii.Popup("ConfigureRetainerSettings" + rowIndex))
        {
            if (popup.Success)
            {
                ImGui.Text("雇员取回：");
                ImGui.Separator();
                if (DrawRetainerRetrievalSelector(configuration, searchResult.CraftItem, rowIndex))
                {
                    ImGui.CloseCurrentPopup();
                }
            }
        }

        using (var popup = ImRaii.Popup("ConfigureMarketWorldPreference" + rowIndex))
        {
            if (popup.Success)
            {
                ImGui.Text("优先市场服务器：");
                ImGui.Separator();
            }
        }

        using (var popup = ImRaii.Popup("ConfigureItemPriceOverride" + rowIndex))
        {
            if (popup.Success)
            {
                ImGui.Text("市场价格：");
                ImGui.Separator();
            }
        }
        var ingredientPreferenceDefault = configuration.CraftList.GetIngredientPreference(searchResult.CraftItem);
        var perItemRetainerRetrieval = configuration.CraftList.GetCraftRetainerRetrieval(searchResult.CraftItem.ItemId);
        var retainerRetrievalDefault = searchResult.CraftItem.IsOutputItem ? configuration.CraftList.CraftRetainerRetrievalOutput : configuration.CraftList.CraftRetainerRetrieval;
        var zonePreference = configuration.CraftList.GetZonePreference(searchResult.CraftItem.IngredientPreference.Type, searchResult.CraftItem.ItemId);
        var worldPreference = configuration.CraftList.GetMarketItemWorldPreference(searchResult.CraftItem.ItemId);
        var priceOverride = configuration.CraftList.GetMarketItemPriceOverride(searchResult.CraftItem.ItemId);
        var originalPos = ImGui.GetCursorPosY();
        DrawRecipeIcon(configuration,rowIndex, searchResult.CraftItem);
        ImGui.SetCursorPosY(originalPos);
        DrawHqIcon(configuration, rowIndex, searchResult.CraftItem);
        ImGui.SetCursorPosY(originalPos);
        DrawRetainerIcon(configuration, rowIndex, searchResult.CraftItem, perItemRetainerRetrieval, retainerRetrievalDefault);
        ImGui.SetCursorPosY(originalPos);

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + configuration.TableHeight / 2.0f - 9);

        var cursorX = ImGui.GetCursorPosX();
        if (ImGuiService.DrawIconButton(_font, FontAwesomeIcon.Cog, ref cursorX))
        {
            ImGui.OpenPopup("ConfigureItemSettings" + columnIndex + searchResult.CraftItem.ItemId + (searchResult.CraftItem.IsOutputItem ? "o" : ""));
        }

        if (ImGui.IsItemHovered())
        {
            using (ImRaii.Tooltip())
            {
                ImGui.TextUnformatted("获取方式：" + (ingredientPreferenceDefault != null ? _ingredientPreferenceLocalizer.FormattedName(ingredientPreferenceDefault) : "使用默认值"));
                ImGui.TextUnformatted("雇员：" + (perItemRetainerRetrieval?.FormattedName() ?? "使用默认值"));
                ImGui.TextUnformatted("区域：" + (zonePreference != null ? _mapSheet.GetRowOrDefault(zonePreference.Value)?.FormattedName ?? "使用默认值" : "使用默认值"));
                if (searchResult.Item.CanBePlacedOnMarket)
                {
                    ImGui.TextUnformatted("市场服务器偏好：" + (worldPreference != null ? (CnWorlds.Names.TryGetValue(worldPreference.Value, out var cnWorldPref) ? cnWorldPref.World : _worldSheet.GetRowOrDefault(worldPreference.Value)?.Name.ExtractText() ?? "使用默认值") : "使用默认值"));
                    ImGui.TextUnformatted("市场价格覆盖：" + (priceOverride != null ? priceOverride.Value.ToString("N0") : "使用默认值"));
                }
            }
        }
        return null;
    }

    public void DrawRetainerIcon(FilterConfiguration configuration, int rowIndex, CraftItem item, CraftRetainerRetrieval? perItemRetainerRetrieval, CraftRetainerRetrieval retainerRetrievalDefault)
    {
        var retainerRetrieval = perItemRetainerRetrieval ?? retainerRetrievalDefault;

        if (retainerRetrievalDefault != perItemRetainerRetrieval)
        {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + configuration.TableHeight / 2.0f - 9);
            ImGui.Image(ImGuiService.GetIconTexture(Icons.RetainerIcon).Handle, new Vector2(20, 20) * ImGui.GetIO().FontGlobalScale,
                new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(1, 1),
                retainerRetrieval == CraftRetainerRetrieval.HqOnly
                    ? new Vector4(0.9f, 0.75f, 0.14f, 1f)
                    : new Vector4(1f, 1f, 1f, 1f));
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
            {
                using (ImRaii.Tooltip())
                {
                    ImGui.Text("雇员取回：");
                    ImGui.Separator();
                    ImGui.Text(retainerRetrieval.FormattedName() + (perItemRetainerRetrieval == null ? "（默认）" : ""));
                }
            }
            ImGui.SameLine();
        }
        else
        {
            ImGui.SetCursorPosY(ImGui.GetCursorPosY() + configuration.TableHeight / 2.0f - 9);
            ImGui.Image(ImGuiService.GetIconTexture(Icons.RetainerIcon).Handle, new Vector2(20, 20) * ImGui.GetIO().FontGlobalScale,
                new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(1, 1), new Vector4(1f, 1f, 1f, 0.2f));
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
            {
                using (ImRaii.Tooltip())
                {
                    ImGui.Text("雇员取回：");
                    ImGui.Separator();
                    ImGui.Text(retainerRetrieval.FormattedName() + "（默认）");
                }
            }
            ImGui.SameLine();
        }
        if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
        {
            CraftRetainerRetrieval? newRetainerRetrieval;
            if (item.Item.IsCollectable)
            {
                if (perItemRetainerRetrieval == CraftRetainerRetrieval.No)
                {
                    newRetainerRetrieval = CraftRetainerRetrieval.Yes;
                }
                else if (perItemRetainerRetrieval == CraftRetainerRetrieval.Yes)
                {
                    newRetainerRetrieval = CraftRetainerRetrieval.CollectableOnly;
                }
                else if (perItemRetainerRetrieval == CraftRetainerRetrieval.CollectableOnly)
                {
                    newRetainerRetrieval = null;
                }
                else
                {
                    newRetainerRetrieval = CraftRetainerRetrieval.No;
                }
            }
            else
            {
                if (perItemRetainerRetrieval == CraftRetainerRetrieval.No)
                {
                    newRetainerRetrieval = CraftRetainerRetrieval.Yes;
                }
                else if (perItemRetainerRetrieval == CraftRetainerRetrieval.Yes)
                {
                    newRetainerRetrieval = CraftRetainerRetrieval.HqOnly;
                }
                else if (perItemRetainerRetrieval == CraftRetainerRetrieval.HqOnly)
                {
                    newRetainerRetrieval = CraftRetainerRetrieval.NqOnly;
                }
                else if (perItemRetainerRetrieval == CraftRetainerRetrieval.NqOnly)
                {
                    newRetainerRetrieval = null;
                }
                else
                {
                    newRetainerRetrieval = CraftRetainerRetrieval.No;
                }
            }

            configuration.NeedsRefresh = true;
            configuration.CraftList.UpdateCraftRetainerRetrieval(item.ItemId, newRetainerRetrieval);
        }
        else if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
        {
            ImGui.OpenPopup("ConfigureRetainerSettings" + rowIndex);
        }
    }

    public void DrawHqIcon(FilterConfiguration configuration, int rowIndex, CraftItem item)
    {
        var hqRequired = configuration.CraftList.GetHQRequired(item.ItemId);
        var isCollectable = item.Item.IsCollectable;

        var calculatedHqRequired = hqRequired ?? configuration.CraftList.HQRequired;

        var canBeHq = item.Item.Base.CanBeHq;

        ImTextureID textureHandle;
        Vector4 iconTint;

        if (isCollectable)
        {
            textureHandle = ImGuiService.GetImageTexture("collectable").Handle;
            iconTint = new Vector4(0.9f, 0.75f, 0.14f, 1f);
        }
        else if (canBeHq && (hqRequired == true || (hqRequired == null && calculatedHqRequired)))
        {
            textureHandle = ImGuiService.GetImageTexture("hq").Handle;
            iconTint = new Vector4(0.9f, 0.75f, 0.14f, 1f);
        }
        else if (hqRequired == false || !canBeHq)
        {
            textureHandle = ImGuiService.GetImageTexture("hq").Handle;
            iconTint = new Vector4(1.0f,1.0f, 1.0f, 0.5f);
        }
        else
        {
            textureHandle = ImGuiService.GetImageTexture("quality").Handle;
            iconTint = new Vector4(1.0f, 1.0f, 1.0f, 1f);
        }

        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + configuration.TableHeight / 2.0f - 9);
        ImGui.Image(textureHandle, new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale,
            new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(1, 1), iconTint);
        if (canBeHq)
        {
            if (ImGui.IsItemClicked(ImGuiMouseButton.Left))
            {
                if (hqRequired == null)
                {
                    configuration.CraftList.UpdateHQRequired(item.ItemId, true);
                    configuration.NeedsRefresh = true;
                }
                else if (hqRequired == true)
                {
                    configuration.CraftList.UpdateHQRequired(item.ItemId, false);
                    configuration.NeedsRefresh = true;
                }
                else
                {
                    configuration.CraftList.UpdateHQRequired(item.ItemId, null);
                    configuration.NeedsRefresh = true;
                }
            }
            else if (item.Item.Base.CanBeHq && ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                ImGui.OpenPopup("ConfigureHQSettings" + rowIndex);
            }
        }
        if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
        {
            using (ImRaii.Tooltip())
            {
                ImGui.Text("物品品质：");
                ImGui.Separator();
                if (isCollectable)
                {
                    ImGui.Text("收藏品");
                }
                else if (hqRequired == true)
                {
                    ImGui.Text("仅HQ（已单独覆盖）");
                }
                else if (hqRequired == false)
                {
                    ImGui.Text("仅NQ（已单独覆盖）");
                }
                else if(canBeHq)
                {
                    ImGui.Text(configuration.CraftList.HQRequired ? "仅HQ（列表默认）" : "优质/普通（列表默认）");
                }
                else
                {
                    ImGui.Text("仅普通品（列表默认）");
                }

                ImGui.Text(canBeHq ? "可为HQ" : "不可为HQ");
                ImGui.Text(isCollectable ? "始终为收藏品" : "不可为收藏品");
            }
        }

        ImGui.SameLine();

    }

    public void DrawRecipeIcon(FilterConfiguration configuration, int rowIndex, CraftItem item)
    {
        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + configuration.TableHeight / 2.0f - 9);
        var icon = _craftItemLocalizer.SourceIcon(item);
        ImGui.Image(ImGuiService.GetIconTexture(icon).Handle,
            new Vector2(18, 18) * ImGui.GetIO().FontGlobalScale);
        var itemRecipe = item.Recipe;
        if (itemRecipe != null)
        {
            if (item.IngredientPreference.Type == IngredientPreferenceType.Crafting && ImGui.IsItemHovered(ImGuiHoveredFlags.None))
            {
                var itemRecipes = item.Item.Recipes.OrderBy(c => c.CraftType?.FormattedName ?? "").ToList();
                if (itemRecipes.Count != 1)
                {
                    if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    {
                        var currentRecipeIndex = itemRecipes.IndexOf(itemRecipe);
                        currentRecipeIndex++;
                        if (itemRecipes.Count <= currentRecipeIndex)
                        {
                            currentRecipeIndex = 0;
                        }

                        var newRecipe = itemRecipes[currentRecipeIndex];
                        if (item.IsOutputItem)
                        {
                            configuration.CraftList.SetCraftRecipe(item.ItemId,
                                newRecipe.RowId);
                            configuration.NeedsRefresh = true;
                        }
                        else
                        {
                            configuration.CraftList.UpdateCraftRecipePreference(item.ItemId,
                                newRecipe.RowId);
                            configuration.NeedsRefresh = true;
                        }
                    }
                    else if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                    {
                        ImGui.OpenPopup("ConfigureRecipeSettings" + rowIndex);
                    }
                }

                using var tt = ImRaii.Tooltip();
        ImGui.Text(string.Format("配方（{0}）：",
            itemRecipe.CraftType?.FormattedName ?? LocalizationService.Ui("Unknown")));
                ImGui.Separator();
                foreach (var ingredient in itemRecipe.IngredientCounts)
                {
                    var actualItem = _itemSheet.GetRowOrDefault(ingredient.Key);
                    var quantity = ingredient.Value;

                    if (actualItem != null)
                    {
                        ImGui.Text(actualItem.NameString + " : " + quantity);
                    }
                }

                if (itemRecipes.Count > 1)
                {
                    ImGui.NewLine();
                    ImGui.Text("左键：下一个配方");
                    ImGui.Text("右键：选择配方");
                }
            }
        }
        else if (item.Item.CompanyCraftSequence != null)
        {
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
            {
                using var tt = ImRaii.Tooltip();
                ImGui.Text("配方（部队制作）：");
                foreach (var ingredient in item.Item.CompanyCraftSequence.MaterialsRequired(item.Phase))
                {
                    var itemId = ingredient.ItemId;
                    var actualItem = _itemSheet.GetRowOrDefault(itemId);
                    var quantity = ingredient.Quantity;
                    if (actualItem != null)
                    {
                        ImGui.Text(actualItem.NameString + " : " + quantity);
                    }
                }
            }
        }
        else if (item.IngredientPreference.Type == IngredientPreferenceType.Item)
        {
            if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
            {
                using var tt = ImRaii.Tooltip();
                ImGui.Text("物品：");
                ImGui.Separator();
                if (item.IngredientPreference.LinkedItemId != null && item.IngredientPreference.LinkedItemQuantity != null)
                {
                    var itemName =
                        _itemSheet.GetRowOrDefault(item.IngredientPreference.LinkedItemId.Value)
                            ?.NameString ?? "未知物品" + " : " + item.IngredientPreference.LinkedItemQuantity.Value;
                    ImGui.Text(itemName);
                    if (item.IngredientPreference.LinkedItem2Id != null &&
                        item.IngredientPreference.LinkedItem2Quantity != null)
                    {
                        var itemName2 =
                            (_itemSheet.GetRowOrDefault(item.IngredientPreference.LinkedItem2Id.Value)
                                ?.NameString ?? "未知物品") + " : " +
                            item.IngredientPreference.LinkedItem2Quantity.Value;
                        ImGui.Text(itemName2);
                    }

                    if (item.IngredientPreference.LinkedItem3Id != null &&
                        item.IngredientPreference.LinkedItem3Quantity != null)
                    {
                        var itemName3 =
                            (_itemSheet.GetRowOrDefault(item.IngredientPreference.LinkedItem3Id.Value)
                                ?.NameString ?? "未知物品") + " : " +
                            item.IngredientPreference.LinkedItem3Quantity.Value;
                        ImGui.Text(itemName3);
                    }
                }
            }
        }
        else
        {
            ImGuiUtil.HoverTooltip(_craftItemLocalizer.SourceName(item));
        }

        ImGui.SameLine();
    }

    public bool DrawSourceSelector(FilterConfiguration configuration, CraftItem item, int rowIndex)
    {
        var ingredientPreferences = _craftingCache.GetIngredientPreferences(item.ItemId);
        if (ingredientPreferences.Count != 0)
        {
            var currentIngredientPreference =
                configuration.CraftList.GetIngredientPreference(item);
            var previewValue = currentIngredientPreference != null ? _ingredientPreferenceLocalizer.FormattedName(currentIngredientPreference) : "使用默认值";
            ImGui.Text("来源偏好：");
            ImGui.SameLine();
            ImGuiService.HelpMarker("物品应如何获取？由于获取方式有多种，你可以依赖制作列表中的素材获取方式设置（制作列表设置内的标签页），也可以在此覆盖获取方式。");
            using (var combo = ImRaii.Combo("##SetIngredients" + rowIndex, previewValue))
            {
                if (combo.Success)
                {
                    if (ImGui.Selectable("使用默认值"))
                    {
                        configuration.CraftList.UpdateIngredientPreference(item.ItemId, null);
                        configuration.NeedsRefresh = true;
                        configuration.NotifyConfigurationChange();
                        return true;
                    }

                    foreach (var ingredientPreference in ingredientPreferences)
                    {
                        if (ImGui.Selectable(_ingredientPreferenceLocalizer.FormattedName(ingredientPreference)))
                        {
                            configuration.CraftList.UpdateIngredientPreference(item.ItemId, ingredientPreference);
                            configuration.NeedsRefresh = true;
                            configuration.NotifyConfigurationChange();
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool DrawMarketPriceSelector(FilterConfiguration configuration, CraftItem item, int rowIndex)
    {
        if (item.IngredientPreference.Type is IngredientPreferenceType.Marketboard)
        {
            var priceOverride = configuration.CraftList.GetMarketItemPriceOverride(item.ItemId);
            var priceString = priceOverride?.ToString() ?? "";
            ImGui.Text("市场价格覆盖：");
            ImGui.SameLine();
            ImGuiService.HelpMarker("覆盖此物品的价格。仅在没有可用价格时使用。可用于粗略估算物品的金币成本。");
            if (ImGui.InputText("##MarketPricePreference" + rowIndex, ref priceString, 50))
            {
                if (priceString == "")
                {
                    configuration.CraftList.UpdateMarketItemPriceOverride(item.ItemId, null);
                    configuration.NeedsRefresh = true;
                }
                else if(UInt32.TryParse(priceString, out uint newValue))
                {
                    configuration.CraftList.UpdateMarketItemPriceOverride(item.ItemId, newValue);
                    configuration.NeedsRefresh = true;
                }
            }
        }

        return false;
    }

    public bool DrawMarketWorldSelector(FilterConfiguration configuration, CraftItem item, int rowIndex)
    {
        if (item.IngredientPreference.Type is IngredientPreferenceType.Marketboard)
        {
            var worldId = configuration.CraftList.GetMarketItemWorldPreference(item.ItemId);
            var currentWorld = worldId != null ? _worldSheet.GetRowOrDefault(worldId.Value) : null;
            var previewValue = (currentWorld != null && CnWorlds.Names.TryGetValue(currentWorld.Value.RowId, out var cnWorld)
                ? cnWorld.World
                : currentWorld?.Name.ExtractText()) ?? "使用默认值";
            ImGui.Text("市场服务器偏好：");
            ImGui.SameLine();
            ImGuiService.HelpMarker("覆盖此物品的市场服务器偏好。若在此选择服务器，制作计价器将优先使用该服务器的价格，然后遵循正常的制作计价规则。");
            using (var combo = ImRaii.Combo("##MarketWorldPreference" + rowIndex, previewValue))
            {
                if (combo.Success)
                {
                    if (ImGui.Selectable("使用默认值"))
                    {
                        configuration.CraftList.UpdateItemWorldPreference(item.ItemId, null);
                        configuration.NeedsRefresh = true;
                        configuration.NotifyConfigurationChange();
                        return true;
                    }
                    foreach (var worldEntry in CnWorlds.Names)
                    {
                        if (ImGui.Selectable($"{worldEntry.Value.DataCenter} · {worldEntry.Value.World}"))
                        {
                            configuration.CraftList.UpdateItemWorldPreference(item.ItemId, worldEntry.Key);
                            configuration.NeedsRefresh = true;
                            configuration.NotifyConfigurationChange();
                            return true;
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool DrawZoneSelector(FilterConfiguration configuration, CraftItem item, int rowIndex)
    {
        if (item.IngredientPreference.Type is IngredientPreferenceType.Buy or IngredientPreferenceType.Item or IngredientPreferenceType.Mobs or IngredientPreferenceType.Mining or IngredientPreferenceType.Botany or IngredientPreferenceType.HouseVendor or IngredientPreferenceType.Duty )
        {
            var mapIds = item.Item.GetSourceMaps(item.IngredientPreference.Type.ToItemInfoTypes(), item.IngredientPreference.LinkedItemId);
            if (mapIds.Count != 0)
            {
                var mapId = configuration.CraftList.GetZonePreference(item.IngredientPreference.Type,item.ItemId);
                var currentMap = mapId != null ? _mapSheet.GetRow(mapId.Value) : null;
                var previewValue = currentMap?.FormattedName ?? "使用默认值";
                ImGui.Text("区域偏好：");
                ImGui.SameLine();
                ImGuiService.HelpMarker("物品应从何处获取？由于有时存在多个获取位置，你可以依赖制作列表中的区域偏好设置（制作列表设置内的标签页），也可以在此覆盖区域。");
                using (var combo = ImRaii.Combo("##ZonePreference" + rowIndex, previewValue))
                {
                    if (combo.Success)
                    {
                        if (ImGui.Selectable("使用默认值"))
                        {
                            configuration.CraftList.UpdateZonePreference(item.IngredientPreference.Type, item.ItemId, null);
                            configuration.NeedsRefresh = true;
                            configuration.NotifyConfigurationChange();
                            return true;
                        }

                        var maps = mapIds.Select(c => _mapSheet.GetRow(c)).Where(c => c != null);
                        foreach (var map in maps)
                        {
                            if (map == null) continue;
                            if (ImGui.Selectable(map.FormattedName))
                            {
                                configuration.CraftList.UpdateZonePreference(item.IngredientPreference.Type, item.ItemId, map.RowId);
                                configuration.NeedsRefresh = true;
                                configuration.NotifyConfigurationChange();
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool DrawRetainerRetrievalSelector(FilterConfiguration configuration, CraftItem item, int rowIndex)
    {
        //Retrieve from retainer combo
        var craftRetainerRetrieval = configuration.CraftList.GetCraftRetainerRetrieval(item.ItemId);
        var previewValue = "使用默认值";
        if (craftRetainerRetrieval != null)
        {
            switch (craftRetainerRetrieval.Value)
            {
                case CraftRetainerRetrieval.Yes:
                    previewValue = "Yes";
                    break;
                case CraftRetainerRetrieval.No:
                    previewValue = "No";
                    break;
                case CraftRetainerRetrieval.HqOnly:
                    previewValue = "仅优质品";
                    break;
                case CraftRetainerRetrieval.NqOnly:
                    previewValue = "仅普通品";
                    break;
                case CraftRetainerRetrieval.CollectableOnly:
                    previewValue = "仅收藏品";
                    break;
            }
        }

        ImGui.Text("雇员取回：");
        ImGui.SameLine();
        ImGuiService.HelpMarker("是否从你的雇员处获取该物品？若有正确品质的数量可用，将显示在雇员/背包中的物品部分。");
        using (var combo = ImRaii.Combo("##SetRetrieveRetainer" + rowIndex, previewValue))
        {
            if (combo.Success)
            {
                if (ImGui.Selectable("使用默认值"))
                {
                    configuration.CraftList.UpdateCraftRetainerRetrieval(item.ItemId, null);
                    configuration.NeedsRefresh = true;
                    configuration.NotifyConfigurationChange();
                    return true;
                }

                if (ImGui.Selectable("是"))
                {
                    configuration.CraftList.UpdateCraftRetainerRetrieval(item.ItemId, CraftRetainerRetrieval.Yes);
                    configuration.NeedsRefresh = true;
                    configuration.NotifyConfigurationChange();
                    return true;
                }

                if (ImGui.Selectable("否"))
                {
                    configuration.CraftList.UpdateCraftRetainerRetrieval(item.ItemId, CraftRetainerRetrieval.No);
                    configuration.NeedsRefresh = true;
                    configuration.NotifyConfigurationChange();
                    return true;
                }

                if (!item.Item.IsCollectable && item.Item.Base.CanBeHq && ImGui.Selectable("仅优质品"))
                {
                    configuration.CraftList.UpdateCraftRetainerRetrieval(item.ItemId, CraftRetainerRetrieval.HqOnly);
                    configuration.NeedsRefresh = true;
                    configuration.NotifyConfigurationChange();
                    return true;
                }

                if (!item.Item.IsCollectable && ImGui.Selectable("仅普通品"))
                {
                    configuration.CraftList.UpdateCraftRetainerRetrieval(item.ItemId, CraftRetainerRetrieval.NqOnly);
                    configuration.NeedsRefresh = true;
                    configuration.NotifyConfigurationChange();
                    return true;
                }

                if (item.Item.IsCollectable && ImGui.Selectable("仅收藏品"))
                {
                    configuration.CraftList.UpdateCraftRetainerRetrieval(item.ItemId, CraftRetainerRetrieval.CollectableOnly);
                    configuration.NeedsRefresh = true;
                    configuration.NotifyConfigurationChange();
                    return true;
                }
            }
        }

        return false;
    }

    public bool DrawHqSelector(FilterConfiguration configuration, CraftItem item, int rowIndex)
    {
        if (item.Item.Base.CanBeHq)
        {
            var currentHQRequired = configuration.CraftList.GetHQRequired(item.ItemId);
            var previewValue = "使用默认值";
            if (currentHQRequired != null)
            {
                previewValue = currentHQRequired.Value ? "Yes" : "No";
            }

            ImGui.Text("需要HQ：");
            ImGui.SameLine();
            ImGuiService.HelpMarker("该物品应为HQ还是NQ？对于成品，只有制作正确品质的物品时所需数量才会减少。对于其他素材，这将决定列出取回的物品以及计入所需数量的部分。");
            using (var combo = ImRaii.Combo("##SetHQRequired" + rowIndex, previewValue))
            {
                if (combo.Success)
                {
                    if (ImGui.Selectable("使用默认值"))
                    {
                        configuration.CraftList.UpdateHQRequired(item.ItemId, null);
                        configuration.NeedsRefresh = true;
                        configuration.NotifyConfigurationChange();
                        return true;
                    }

                    if (ImGui.Selectable("是"))
                    {
                        configuration.CraftList.UpdateHQRequired(item.ItemId, true);
                        configuration.NeedsRefresh = true;
                        configuration.NotifyConfigurationChange();
                        return true;
                    }

                    if (ImGui.Selectable("否"))
                    {
                        configuration.CraftList.UpdateHQRequired(item.ItemId, false);
                        configuration.NeedsRefresh = true;
                        configuration.NotifyConfigurationChange();
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool DrawRecipeSelector(FilterConfiguration configuration, CraftItem item, int rowIndex)
    {
        var itemRecipes = item.Item.Recipes.OrderBy(c => c.CraftType?.FormattedName ?? "").ToList();
        if (itemRecipes.Count > 1)
        {
            var recipeName = item.Recipe?.CraftType?.FormattedName ?? "";
            ImGui.Text("配方：");
            ImGui.SameLine();
            ImGuiService.HelpMarker("选择用于该物品的配方。某些物品可由多个职业制作。");
            using (var combo = ImRaii.Combo("##SetRecipe" + rowIndex, recipeName))
            {
                if (combo.Success)
                {
                    foreach (var recipe in itemRecipes)
                    {
                        if (ImGui.Selectable(recipe.CraftType?.FormattedName ?? "",
                                recipeName == (recipe.CraftType?.FormattedName ?? "")))
                        {
                            if (item.IsOutputItem)
                            {
                                configuration.CraftList.SetCraftRecipe(item.ItemId,
                                    recipe.RowId);
                                configuration.NeedsRefresh = true;
                                return true;
                            }
                            else
                            {
                                configuration.CraftList.UpdateCraftRecipePreference(item.ItemId,
                                    recipe.RowId);
                                configuration.NeedsRefresh = true;
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public List<MessageBase>? DrawEditor(ColumnConfiguration columnConfiguration, FilterConfiguration configuration)
    {
        return null;
    }

    public string CsvExport(ColumnConfiguration columnConfiguration, SearchResult item)
    {
        return "";
    }

    public dynamic? JsonExport(ColumnConfiguration columnConfiguration, SearchResult item)
    {
        return "";
    }

    public void Setup(FilterConfiguration filterConfiguration, ColumnConfiguration columnConfiguration, int columnIndex)
    {
        ImGui.TableSetupColumn(columnConfiguration.Name ?? (RenderName ?? Name), ImGuiTableColumnFlags.WidthFixed, Width, (uint)columnIndex);
    }

    public bool? DrawFilter(ColumnConfiguration columnConfiguration, int columnIndex)
    {
        return null;
    }

    public IFilterEvent? DrawFooterFilter(ColumnConfiguration columnConfiguration, FilterTable filterTable)
    {
        return null;
    }

    public event IColumn.ButtonPressedDelegate? ButtonPressed;

    public virtual void InvalidateSearchCache()
    {

    }

    public void Dispose()
    {
    }
    public virtual FilterType DefaultIn => Logic.FilterType.CraftFilter;
    public int MaxFilterLength { get; set; } = 200;
}
