using System.Collections.Generic;
using System.Numerics;
using AllaganLib.GameSheets.Caches;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using DalaMock.Shared.Interfaces;
using Dalamud.Interface;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Ui.Widgets;
using Dalamud.Interface.Utility.Raii;
using InventoryTools.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class CraftSimpleColumn : TextColumn
    {
        private readonly IFont _font;
        private readonly ExcelSheet<World> _worldSheet;
        private readonly ExcelSheet<Item> _itemSheet;

        public CraftSimpleColumn(ILogger<CraftSimpleColumn> logger, ImGuiService imGuiService, IFont font, ExcelSheet<World> worldSheet, ExcelSheet<Item> itemSheet) : base(logger, imGuiService)
        {
            _font = font;
            _worldSheet = worldSheet;
            _itemSheet = itemSheet;
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Crafting;
        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return "";
        }

        public override List<MessageBase>? Draw(FilterConfiguration configuration,
            ColumnConfiguration columnConfiguration,
            SearchResult searchResult, int rowIndex, int columnIndex)
        {
            if (searchResult.CraftItem == null) return null;

            ImGui.TableNextColumn();
            if (!ImGui.TableGetColumnFlags().HasFlag(ImGuiTableColumnFlags.IsEnabled)) return null;
            var originalCursorPosY = ImGui.GetCursorPosY();
            var nextStep = configuration.CraftList.GetNextStep(searchResult.CraftItem);
            ImGuiUtil.VerticalAlignTextColored(nextStep.Item2, nextStep.Item1, configuration.TableHeight, true);
            if (searchResult.CraftItem.IsOutputItem && searchResult.CraftItem.IsCompleted)
            {
                ImGui.SameLine();
                ImGui.SetCursorPosY(ImGui.GetCursorPosY() + configuration.TableHeight / 2.0f - 9);
                ImGui.Image(ImGuiService.GetIconTexture(Icons.RedXIcon).Handle, new Vector2(20, 20) * ImGui.GetIO().FontGlobalScale,
                    new System.Numerics.Vector2(0, 0), new System.Numerics.Vector2(1, 1));
                if (ImGui.IsItemHovered())
                {
                    ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
                }

                if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    configuration.CraftList.RemoveCraftItem(searchResult.CraftItem.ItemId);
                    configuration.NeedsRefresh = true;
                }
                OtterGui.ImGuiUtil.HoverTooltip("删除物品");

            }

            if (searchResult.CraftItem.IngredientPreference.Type == IngredientPreferenceType.Marketboard)
            {
                if (searchResult.CraftItem.MarketTotalPrice != null && searchResult.CraftItem.MarketUnitPrice != null)
                {
                    if (searchResult.Item.HasSourcesByType(ItemInfoType.GilShop, ItemInfoType.CalamitySalvagerShop))
                    {
                        var currentIndex = configuration.CraftList.IngredientPreferenceTypeOrder.IndexOf((
                            searchResult.CraftItem.IngredientPreference.Type,
                            searchResult.CraftItem.IngredientPreference.LinkedItemId));
                        var buyIndex =
                            configuration.CraftList.IngredientPreferenceTypeOrder.IndexOf((IngredientPreferenceType.Buy, null));
                        var houseVendorIndex =
                            configuration.CraftList.IngredientPreferenceTypeOrder.IndexOf((IngredientPreferenceType.HouseVendor,
                                null));
                        if (currentIndex < buyIndex || currentIndex < houseVendorIndex)
                        {
                            if (searchResult.CraftItem.MarketUnitPrice != 0 && searchResult.CraftItem.MarketUnitPrice < searchResult.Item.BuyFromVendorPrice)
                            {
                                ImGui.SameLine();
                                ImGui.Image(ImGuiService.GetIconTexture(Icons.QuestionMarkIcon).Handle, new Vector2(16, 16));
                                OtterGui.ImGuiUtil.HoverTooltip(
                                    "该物品的市场价格比从商店购买更便宜，且你更喜欢商店而不是当前的素材偏好。");
                            }
                        }
                    }
                }
                else
                {
                    ImGui.Text("不适用");
                }

                var craftPrices = searchResult.CraftItem.CraftPrices;
                if (craftPrices != null && craftPrices.Count != 0)
                {
                    ImGui.SameLine();
                    ImGui.Image(ImGuiService.GetIconTexture(Icons.MarketboardIcon).Handle, new Vector2(16,16));
                    if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
                    {
                        using (ImRaii.Tooltip())
                        {
                            var totalAvailable = 0;
                            foreach (var price in craftPrices)
                            {
                                var world = _worldSheet.GetRowOrDefault(price.WorldId);
                                if (world != null)
                                {
                                    ImGui.Text(price.Left + " 可在 " + price.UnitPrice +
                                               (price.IsHq ? " (HQ)" : "") + " (" + world.Value.Name.ExtractText() + ")");
                                }

                                totalAvailable += price.Left;
                            }

                            ImGui.Text("可用：" + totalAvailable);

                            if (searchResult.CraftItem.MarketAvailable != searchResult.CraftItem.QuantityNeeded)
                            {
                                ImGui.Text("缺少：" + (searchResult.CraftItem.QuantityNeeded - searchResult.CraftItem.MarketAvailable));
                            }
                        }
                    }
                }

            }
            else if (searchResult.CraftItem.MissingIngredients.Count != 0)
            {
                ImGui.SameLine();
                ImGui.SetCursorPosY(originalCursorPosY);
                ImGui.PushFont(_font.IconFont);
                ImGuiUtil.VerticalAlignTextDisabled(FontAwesomeIcon.InfoCircle.ToIconString(), configuration.TableHeight, false);
                ImGui.PopFont();
                if (ImGui.IsItemHovered(ImGuiHoveredFlags.None))
                {
                    using var tt = ImRaii.Tooltip();
                    ImGui.Text("缺少的素材：");
                    foreach (var missingIngredient in searchResult.CraftItem.MissingIngredients)
                    {
                        var itemId = missingIngredient.Key.Item1;
                        var quantity = missingIngredient.Value;
                        var isHq = missingIngredient.Key.Item2;
                        var actualItem = _itemSheet.GetRowOrDefault(itemId);
                        if (actualItem != null)
                        {
                            ImGui.Text(actualItem.Value.Name.ExtractText() + " : " + quantity);
                        }
                    }
                }
            }

            return null;
        }

        public override string Name { get; set; } = "制作下一步";
        public override string RenderName => "下一步";

        public override float Width { get; set; } = 200;
        public override bool? CraftOnly => true;

        public override string HelpText { get; set; } =
            "显示制作下一步的简化提示";
        public override bool HasFilter { get; set; } = false;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override FilterType AvailableIn { get; } = Logic.FilterType.CraftFilter;
        public override FilterType DefaultIn => Logic.FilterType.CraftFilter;
    }
}