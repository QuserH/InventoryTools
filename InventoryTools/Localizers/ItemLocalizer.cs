using System.Collections.Generic;
using CriticalCommonLib.Enums;
using CriticalCommonLib.Models;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using InventoryTools.Localization;

namespace InventoryTools.Localizers;

public class ItemLocalizer
{
    private readonly ExcelSheet<Addon> _addonSheet;
    private Dictionary<uint, string> _cabinetNames;

    public ItemLocalizer(ExcelSheet<Addon> addonSheet)
    {
        _addonSheet = addonSheet;
        _cabinetNames = new();
    }

    public string CabinetName(InventoryItem inventoryItem)
    {
        if (inventoryItem.SortedContainer != InventoryType.Armoire)
        {
            return "";
        }

        var cabinetCategory = inventoryItem.Item.CabinetCategory;
        if (cabinetCategory == null)
        {
            return "未知柜子";
        }

        if (_cabinetNames.TryGetValue(cabinetCategory.Base.Category.RowId, out string? cabinetName))
        {
            return cabinetName;
        }

        cabinetName = _addonSheet.GetRowOrDefault(cabinetCategory.Base.Category.RowId)?.Text.ExtractText() ??
                      "未找到 Addon 文本";

        _cabinetNames[cabinetCategory.Base.Category.RowId] = cabinetName;

        return cabinetName;
    }

    public string ItemDescription(InventoryItem inventoryItem)
    {
        if (inventoryItem.IsEmpty)
        {
            return "空";
        }

        var _item = inventoryItem.Item.NameString.ToString();
        if (inventoryItem.IsHQ)
        {
            _item += " (HQ)";
        }
        else if (inventoryItem.IsCollectible)
        {
            _item += "（收藏品）";
        }
        else
        {
            _item += " (NQ)";
        }

        if (inventoryItem.SortedCategory == InventoryCategory.Currency)
        {
            _item += " - " + SortedContainerName(inventoryItem);
        }
        else
        {
            _item += " - " + SortedContainerName(inventoryItem) + " - " + (inventoryItem.SortedSlotIndex + 1);
        }


        return _item;
    }

    public string FormattedBagLocation(InventoryItem inventoryItem)
    {
        if (inventoryItem.SortedContainer is InventoryType.GlamourChest or InventoryType.Currency or InventoryType.RetainerGil or InventoryType.FreeCompanyGil or InventoryType.Crystal or InventoryType.RetainerCrystal)
        {
            return SortedContainerName(inventoryItem);
        }
        return SortedContainerName(inventoryItem) + " - " + (inventoryItem.SortedSlotIndex + 1);
    }

    public string SortedContainerName(InventoryItem inventoryItem)
    {
        if(inventoryItem.SortedContainer is InventoryType.Bag0 or InventoryType.RetainerBag0)
        {
            return "背包 1";
        }
        if(inventoryItem.SortedContainer is InventoryType.Bag1 or InventoryType.RetainerBag1)
        {
            return "背包 2";
        }
        if(inventoryItem.SortedContainer is InventoryType.Bag2 or InventoryType.RetainerBag2)
        {
            return "背包 3";
        }
        if(inventoryItem.SortedContainer is InventoryType.Bag3 or InventoryType.RetainerBag3)
        {
            return "背包 4";
        }
        if(inventoryItem.SortedContainer is InventoryType.RetainerBag4)
        {
            return "背包 5";
        }
        if(inventoryItem.SortedContainer is InventoryType.SaddleBag0)
        {
            return "陆行鸟鞍囊 左";
        }
        if(inventoryItem.SortedContainer is InventoryType.SaddleBag1)
        {
            return "陆行鸟鞍囊 右";
        }
        if(inventoryItem.SortedContainer is InventoryType.PremiumSaddleBag0)
        {
            return "高级陆行鸟鞍囊 左";
        }
        if(inventoryItem.SortedContainer is InventoryType.PremiumSaddleBag1)
        {
            return "高级陆行鸟鞍囊 右";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryBody)
        {
            return "兵装库 - 身体";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryEar)
        {
            return "兵装库 - 耳部";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryFeet)
        {
            return "兵装库 - 脚部";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryHand)
        {
            return "兵装库 - 手部";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryHead)
        {
            return "兵装库 - 头部";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryLegs)
        {
            return "兵装库 - 腿部";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryMain)
        {
            return "兵装库 - 主手";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryNeck)
        {
            return "兵装库 - 颈部";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryOff)
        {
            return "兵装库 - 副手";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryRing)
        {
            return "兵装库 - 戒指";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryWaist)
        {
            return "兵装库 - 腰部";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryWrist)
        {
            return "兵装库 - 腕部";
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmorySoulCrystal)
        {
            return "兵装库 - 特职水晶";
        }
        if(inventoryItem.SortedContainer is InventoryType.GearSet0)
        {
            return "已装备的装备";
        }
        if(inventoryItem.SortedContainer is InventoryType.RetainerEquippedGear)
        {
            return "已装备的装备";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag0)
        {
            return "部队储物柜 - 1";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag1)
        {
            return "部队储物柜 - 2";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag2)
        {
            return "部队储物柜 - 3";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag3)
        {
            return "部队储物柜 - 4";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag4)
        {
            return "部队储物柜 - 5";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag5)
        {
            return "部队储物柜 - 6";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag6)
        {
            return "部队储物柜 - 7";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag7)
        {
            return "部队储物柜 - 8";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag8)
        {
            return "部队储物柜 - 9";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag9)
        {
            return "部队储物柜 - 10";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag10)
        {
            return "部队储物柜 - 11";
        }
        if(inventoryItem.SortedContainer is InventoryType.RetainerMarket)
        {
            return "市场";
        }
        if(inventoryItem.SortedContainer is InventoryType.GlamourChest)
        {
            return "幻化柜";
        }
        if(inventoryItem.SortedContainer is InventoryType.Armoire)
        {
            return "收藏柜 - " + CabinetName(inventoryItem);
        }
        if(inventoryItem.SortedContainer is InventoryType.Currency)
        {
            return "货币";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyGil)
        {
            return "部队 - 金币";
        }
        if(inventoryItem.SortedContainer is InventoryType.RetainerGil)
        {
            return "货币";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyCrystal)
        {
            return "部队 - 水晶";
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyCurrency)
        {
            return "部队 - 货币";
        }
        if(inventoryItem.SortedContainer is InventoryType.Crystal or InventoryType.RetainerCrystal)
        {
            return "水晶";
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingExteriorAppearance)
        {
            return "房屋外观";
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingInteriorAppearance)
        {
            return "房屋室内外观";
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingExteriorStoreroom or InventoryType.HousingExteriorStoreroom2)
        {
            return "房屋外部储物间";
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingInteriorStoreroom1 or InventoryType.HousingInteriorStoreroom2 or InventoryType.HousingInteriorStoreroom3 or InventoryType.HousingInteriorStoreroom4 or InventoryType.HousingInteriorStoreroom5 or InventoryType.HousingInteriorStoreroom6 or InventoryType.HousingInteriorStoreroom7 or InventoryType.HousingInteriorStoreroom8 or InventoryType.HousingInteriorStoreroom9 or InventoryType.HousingInteriorStoreroom10 or InventoryType.HousingInteriorStoreroom11)
        {
            return "房屋室内储物间";
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingInteriorPlacedItems1 or InventoryType.HousingInteriorPlacedItems2 or InventoryType.HousingInteriorPlacedItems3 or InventoryType.HousingInteriorPlacedItems4 or InventoryType.HousingInteriorPlacedItems5 or InventoryType.HousingInteriorPlacedItems6 or InventoryType.HousingInteriorPlacedItems7 or InventoryType.HousingInteriorPlacedItems8 or InventoryType.HousingInteriorPlacedItems9 or InventoryType.HousingInteriorPlacedItems10 or InventoryType.HousingInteriorPlacedItems11 or InventoryType.HousingInteriorPlacedItems12)
        {
            return "房屋室内放置物品";
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingExteriorPlacedItems or InventoryType.HousingExteriorPlacedItems2)
        {
            return "房屋外部放置物品";
        }

        return CriticalCommonLib.Localization.SharedLocalization.Get(inventoryItem.SortedContainer.ToString());
    }
}