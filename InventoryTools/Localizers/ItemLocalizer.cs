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
            return LocalizationService.Ui("Unknown Cabinet");
        }

        if (_cabinetNames.TryGetValue(cabinetCategory.Base.Category.RowId, out string? cabinetName))
        {
            return cabinetName;
        }

        cabinetName = _addonSheet.GetRowOrDefault(cabinetCategory.Base.Category.RowId)?.Text.ExtractText() ??
                      LocalizationService.Ui("Addon Text Not Found");

        _cabinetNames[cabinetCategory.Base.Category.RowId] = cabinetName;

        return cabinetName;
    }

    public string ItemDescription(InventoryItem inventoryItem)
    {
        if (inventoryItem.IsEmpty)
        {
            return "Empty";
        }

        var _item = inventoryItem.Item.NameString.ToString();
        if (inventoryItem.IsHQ)
        {
            _item += LocalizationService.Ui(" (HQ)");
        }
        else if (inventoryItem.IsCollectible)
        {
            _item += LocalizationService.Ui(" (Collectible)");
        }
        else
        {
            _item += LocalizationService.Ui(" (NQ)");
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
            return LocalizationService.Ui("Bag 1");
        }
        if(inventoryItem.SortedContainer is InventoryType.Bag1 or InventoryType.RetainerBag1)
        {
            return LocalizationService.Ui("Bag 2");
        }
        if(inventoryItem.SortedContainer is InventoryType.Bag2 or InventoryType.RetainerBag2)
        {
            return LocalizationService.Ui("Bag 3");
        }
        if(inventoryItem.SortedContainer is InventoryType.Bag3 or InventoryType.RetainerBag3)
        {
            return LocalizationService.Ui("Bag 4");
        }
        if(inventoryItem.SortedContainer is InventoryType.RetainerBag4)
        {
            return LocalizationService.Ui("Bag 5");
        }
        if(inventoryItem.SortedContainer is InventoryType.SaddleBag0)
        {
            return LocalizationService.Ui("Saddlebag Left");
        }
        if(inventoryItem.SortedContainer is InventoryType.SaddleBag1)
        {
            return LocalizationService.Ui("Saddlebag Right");
        }
        if(inventoryItem.SortedContainer is InventoryType.PremiumSaddleBag0)
        {
            return LocalizationService.Ui("Premium Saddlebag Left");
        }
        if(inventoryItem.SortedContainer is InventoryType.PremiumSaddleBag1)
        {
            return LocalizationService.Ui("Premium Saddlebag Right");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryBody)
        {
            return LocalizationService.Ui("Armory - Body");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryEar)
        {
            return LocalizationService.Ui("Armory - Ear");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryFeet)
        {
            return LocalizationService.Ui("Armory - Feet");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryHand)
        {
            return LocalizationService.Ui("Armory - Hand");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryHead)
        {
            return LocalizationService.Ui("Armory - Head");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryLegs)
        {
            return LocalizationService.Ui("Armory - Legs");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryMain)
        {
            return LocalizationService.Ui("Armory - Main");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryNeck)
        {
            return LocalizationService.Ui("Armory - Neck");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryOff)
        {
            return LocalizationService.Ui("Armory - Offhand");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryRing)
        {
            return LocalizationService.Ui("Armory - Ring");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryWaist)
        {
            return LocalizationService.Ui("Armory - Waist");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmoryWrist)
        {
            return LocalizationService.Ui("Armory - Wrist");
        }
        if(inventoryItem.SortedContainer is InventoryType.ArmorySoulCrystal)
        {
            return LocalizationService.Ui("Armory - Soul Crystal");
        }
        if(inventoryItem.SortedContainer is InventoryType.GearSet0)
        {
            return LocalizationService.Ui("Equipped Gear");
        }
        if(inventoryItem.SortedContainer is InventoryType.RetainerEquippedGear)
        {
            return LocalizationService.Ui("Equipped Gear");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag0)
        {
            return LocalizationService.Ui("Free Company Chest - 1");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag1)
        {
            return LocalizationService.Ui("Free Company Chest - 2");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag2)
        {
            return LocalizationService.Ui("Free Company Chest - 3");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag3)
        {
            return LocalizationService.Ui("Free Company Chest - 4");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag4)
        {
            return LocalizationService.Ui("Free Company Chest - 5");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag5)
        {
            return LocalizationService.Ui("Free Company Chest - 6");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag6)
        {
            return LocalizationService.Ui("Free Company Chest - 7");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag7)
        {
            return LocalizationService.Ui("Free Company Chest - 8");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag8)
        {
            return LocalizationService.Ui("Free Company Chest - 9");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag9)
        {
            return LocalizationService.Ui("Free Company Chest - 10");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyBag10)
        {
            return LocalizationService.Ui("Free Company Chest - 11");
        }
        if(inventoryItem.SortedContainer is InventoryType.RetainerMarket)
        {
            return LocalizationService.Ui("Market");
        }
        if(inventoryItem.SortedContainer is InventoryType.GlamourChest)
        {
            return LocalizationService.Ui("Glamour Chest");
        }
        if(inventoryItem.SortedContainer is InventoryType.Armoire)
        {
            return LocalizationService.Ui("Armoire - ") + CabinetName(inventoryItem);
        }
        if(inventoryItem.SortedContainer is InventoryType.Currency)
        {
            return LocalizationService.Ui("Currency");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyGil)
        {
            return LocalizationService.Ui("Free Company - Gil");
        }
        if(inventoryItem.SortedContainer is InventoryType.RetainerGil)
        {
            return LocalizationService.Ui("Currency");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyCrystal)
        {
            return LocalizationService.Ui("Free Company - Crystals");
        }
        if(inventoryItem.SortedContainer is InventoryType.FreeCompanyCurrency)
        {
            return LocalizationService.Ui("Free Company - Currency");
        }
        if(inventoryItem.SortedContainer is InventoryType.Crystal or InventoryType.RetainerCrystal)
        {
            return LocalizationService.Ui("Crystals");
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingExteriorAppearance)
        {
            return LocalizationService.Ui("Housing Exterior Appearance");
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingInteriorAppearance)
        {
            return LocalizationService.Ui("Housing Interior Appearance");
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingExteriorStoreroom or InventoryType.HousingExteriorStoreroom2)
        {
            return LocalizationService.Ui("Housing Exterior Storeroom");
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingInteriorStoreroom1 or InventoryType.HousingInteriorStoreroom2 or InventoryType.HousingInteriorStoreroom3 or InventoryType.HousingInteriorStoreroom4 or InventoryType.HousingInteriorStoreroom5 or InventoryType.HousingInteriorStoreroom6 or InventoryType.HousingInteriorStoreroom7 or InventoryType.HousingInteriorStoreroom8 or InventoryType.HousingInteriorStoreroom9 or InventoryType.HousingInteriorStoreroom10 or InventoryType.HousingInteriorStoreroom11)
        {
            return LocalizationService.Ui("Housing Interior Storeroom");
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingInteriorPlacedItems1 or InventoryType.HousingInteriorPlacedItems2 or InventoryType.HousingInteriorPlacedItems3 or InventoryType.HousingInteriorPlacedItems4 or InventoryType.HousingInteriorPlacedItems5 or InventoryType.HousingInteriorPlacedItems6 or InventoryType.HousingInteriorPlacedItems7 or InventoryType.HousingInteriorPlacedItems8 or InventoryType.HousingInteriorPlacedItems9 or InventoryType.HousingInteriorPlacedItems10 or InventoryType.HousingInteriorPlacedItems11 or InventoryType.HousingInteriorPlacedItems12)
        {
            return LocalizationService.Ui("Housing Interior Placed Items");
        }
        if(inventoryItem.SortedContainer is InventoryType.HousingExteriorPlacedItems or InventoryType.HousingExteriorPlacedItems2)
        {
            return LocalizationService.Ui("Housing Exterior Placed Items");
        }

        return CriticalCommonLib.Localization.SharedLocalization.Get(inventoryItem.SortedContainer.ToString());
    }
}