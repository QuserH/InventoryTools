using System.Collections.Generic;
using System.Linq;
using AllaganLib.Shared.Interfaces;
using CriticalCommonLib;
using CriticalCommonLib.Services;
using Dalamud.Bindings.ImGui;
using FFXIVClientStructs.FFXIV.Client.Game;
using InventoryTools.Localization;

namespace InventoryTools.Debuggers;

public class InventoryScannerDebuggerPane : IDebugPane
{
    private readonly InventoryScanner _inventoryScanner;

    public InventoryScannerDebuggerPane(InventoryScanner inventoryScanner)
    {
        _inventoryScanner = inventoryScanner;
    }
    public string Name => "背包扫描器";
    public void Draw()
    {
        ImGui.TextUnformatted("通过网络数据可见的背包");
        foreach (var inventory in _inventoryScanner.LoadedInventories)
        {
            ImGui.TextUnformatted(inventory.ToString());
        }

        ImGui.TextUnformatted("通过网络数据可见的雇员背包");
        foreach (var inventory in _inventoryScanner.InMemoryRetainers)
        {
            ImGui.TextUnformatted(inventory.Key.ToString());
            foreach (var hashSet in inventory.Value)
            {
                ImGui.TextUnformatted(hashSet.ToString());
            }
        }
        if (ImGui.TreeNode("角色背包 1##characterBags1"))
        {
            for (int i = 0; i < _inventoryScanner.CharacterBag1.Length; i++)
            {
                var item = _inventoryScanner.CharacterBag1[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("角色背包 2##characterBags2"))
        {
            for (int i = 0; i < _inventoryScanner.CharacterBag2.Length; i++)
            {
                var item = _inventoryScanner.CharacterBag2[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("角色背包 3##characterBags3"))
        {
            for (int i = 0; i < _inventoryScanner.CharacterBag3.Length; i++)
            {
                var item = _inventoryScanner.CharacterBag3[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("角色背包 4##characterBags4"))
        {
            for (int i = 0; i < _inventoryScanner.CharacterBag4.Length; i++)
            {
                var item = _inventoryScanner.CharacterBag4[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("角色已装备##characterEquipped"))
        {
            for (int i = 0; i < _inventoryScanner.CharacterEquipped.Length; i++)
            {
                var item = _inventoryScanner.CharacterEquipped[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("角色水晶##characterCrystals"))
        {
            for (int i = 0; i < _inventoryScanner.CharacterCrystals.Length; i++)
            {
                var item = _inventoryScanner.CharacterCrystals[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("角色货币##characterCurrency"))
        {
            for (int i = 0; i < _inventoryScanner.CharacterCurrency.Length; i++)
            {
                var item = _inventoryScanner.CharacterCurrency[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("陆行鸟鞍囊 左##saddlebagLeft"))
        {
            for (int i = 0; i < _inventoryScanner.SaddleBag1.Length; i++)
            {
                var item = _inventoryScanner.SaddleBag1[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("陆行鸟鞍囊 右##saddlebagRight"))
        {
            for (int i = 0; i < _inventoryScanner.SaddleBag2.Length; i++)
            {
                var item = _inventoryScanner.SaddleBag2[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("高级陆行鸟鞍囊 左##premiumSaddleBagLeft"))
        {
            for (int i = 0; i < _inventoryScanner.PremiumSaddleBag1.Length; i++)
            {
                var item = _inventoryScanner.PremiumSaddleBag1[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("高级陆行鸟鞍囊 右##premiumSaddleBagRight"))
        {
            for (int i = 0; i < _inventoryScanner.PremiumSaddleBag2.Length; i++)
            {
                var item = _inventoryScanner.PremiumSaddleBag2[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 头部##armouryHead"))
        {
            for (int i = 0; i < _inventoryScanner.ArmouryHead.Length; i++)
            {
                var item = _inventoryScanner.ArmouryHead[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 主手##armouryMainHand"))
        {
            for (int i = 0; i < _inventoryScanner.ArmouryMainHand.Length; i++)
            {
                var item = _inventoryScanner.ArmouryMainHand[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 身体##armouryBody"))
        {
            for (int i = 0; i < _inventoryScanner.ArmouryBody.Length; i++)
            {
                var item = _inventoryScanner.ArmouryBody[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 手部##armouryHands"))
        {
            for (int i = 0; i < _inventoryScanner.ArmouryHands.Length; i++)
            {
                var item = _inventoryScanner.ArmouryHands[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 腿部##armouryLegs"))
        {
            for (int i = 0; i < _inventoryScanner.ArmouryLegs.Length; i++)
            {
                var item = _inventoryScanner.ArmouryLegs[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 脚部##armouryFeet"))
        {
            for (int i = 0; i < _inventoryScanner.ArmouryFeet.Length; i++)
            {
                var item = _inventoryScanner.ArmouryFeet[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 副手##armouryOffHand"))
        {
            for (int i = 0; i < _inventoryScanner.ArmouryOffHand.Length; i++)
            {
                var item = _inventoryScanner.ArmouryOffHand[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 耳部##armouryEars"))
        {
            for (int i = 0; i < _inventoryScanner.ArmouryEars.Length; i++)
            {
                var item = _inventoryScanner.ArmouryEars[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 颈部##armouryNeck"))
        {
            for (int i = 0; i < _inventoryScanner.ArmouryNeck.Length; i++)
            {
                var item = _inventoryScanner.ArmouryNeck[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 腕部##armouryWrists"))
        {
            for (int i = 0; i < _inventoryScanner.ArmouryWrists.Length; i++)
            {
                var item = _inventoryScanner.ArmouryWrists[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 戒指##armouryRings"))
        {
            for (int i = 0; i < _inventoryScanner.ArmouryRings.Length; i++)
            {
                var item = _inventoryScanner.ArmouryRings[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("兵装库 - 特职水晶##armourySoulCrystals"))
        {
            for (int i = 0; i < _inventoryScanner.ArmourySoulCrystals.Length; i++)
            {
                var item = _inventoryScanner.ArmourySoulCrystals[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("部队储物柜 1##freeCompanyBags1"))
        {
            for (int i = 0; i < _inventoryScanner.FreeCompanyBag1.Length; i++)
            {
                var item = _inventoryScanner.FreeCompanyBag1[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("部队储物柜 2##freeCompanyBags2"))
        {
            for (int i = 0; i < _inventoryScanner.FreeCompanyBag2.Length; i++)
            {
                var item = _inventoryScanner.FreeCompanyBag2[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("部队储物柜 3##freeCompanyBags3"))
        {
            for (int i = 0; i < _inventoryScanner.FreeCompanyBag3.Length; i++)
            {
                var item = _inventoryScanner.FreeCompanyBag3[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("部队储物柜 4##freeCompanyBags4"))
        {
            for (int i = 0; i < _inventoryScanner.FreeCompanyBag4.Length; i++)
            {
                var item = _inventoryScanner.FreeCompanyBag4[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("部队储物柜 5##freeCompanyBags5"))
        {
            for (int i = 0; i < _inventoryScanner.FreeCompanyBag5.Length; i++)
            {
                var item = _inventoryScanner.FreeCompanyBag5[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("部队货币##freeCompanyCurrency"))
        {
            var bagType = (InventoryType)CriticalCommonLib.Enums.InventoryType.FreeCompanyCurrency;
            var bag = _inventoryScanner.GetInventoryByType(bagType);
            var bagLoaded = _inventoryScanner.IsBagLoaded(bagType);
            if (ImGui.TreeNode(bagType.ToString() + (bagLoaded ? " (Loaded)" : " (Not Loaded)")))
            {
                var itemCount = bag.Count(c => c.ItemId != 0);
                ImGui.Text(itemCount + "/" + bag.Length);
                for (int i = 0; i < bag.Length; i++)
                {
                    var item = bag[i];
                    Utils.PrintOutObject(item, (ulong)i, new List<string>());
                }

                ImGui.TreePop();
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("收藏柜##armoire"))
        {
            for (int i = 0; i < _inventoryScanner.Armoire.Length; i++)
            {
                var item = _inventoryScanner.Armoire[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("幻化柜##glamourChest"))
        {
            for (int i = 0; i < _inventoryScanner.GlamourChest.Length; i++)
            {
                var item = _inventoryScanner.GlamourChest[i];
                Utils.PrintOutObject(item, (ulong)i, new List<string>());
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("雇员背包 1##retainerBag1"))
        {
            foreach (var retainer in _inventoryScanner.RetainerBag1)
            {
                if (ImGui.TreeNode("雇员背包 " + retainer.Key + "##1" + retainer.Key))
                {
                    for (int i = 0; i < retainer.Value.Length; i++)
                    {
                        var item = retainer.Value[i];
                        Utils.PrintOutObject(item, (ulong)i, new List<string>());
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("雇员背包 2##retainerBag2"))
        {
            foreach (var retainer in _inventoryScanner.RetainerBag2)
            {
                if (ImGui.TreeNode("雇员背包 " + retainer.Key + "##2" + retainer.Key))
                {
                    for (int i = 0; i < retainer.Value.Length; i++)
                    {
                        var item = retainer.Value[i];
                        Utils.PrintOutObject(item, (ulong)i, new List<string>());
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("雇员背包 3##retainerBag3"))
        {
            foreach (var retainer in _inventoryScanner.RetainerBag3)
            {
                if (ImGui.TreeNode("雇员背包 " + retainer.Key + "##3" + retainer.Key))
                {
                    for (int i = 0; i < retainer.Value.Length; i++)
                    {
                        var item = retainer.Value[i];
                        Utils.PrintOutObject(item, (ulong)i, new List<string>());
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("雇员背包 4##retainerBag4"))
        {
            foreach (var retainer in _inventoryScanner.RetainerBag4)
            {
                if (ImGui.TreeNode("雇员背包 " + retainer.Key + "##4" + retainer.Key))
                {
                    for (int i = 0; i < retainer.Value.Length; i++)
                    {
                        var item = retainer.Value[i];
                        Utils.PrintOutObject(item, (ulong)i, new List<string>());
                    }

                    ImGui.TreePop();
                }
            }
            ImGui.TreePop();
        }

        if (ImGui.TreeNode("雇员背包 5##retainerBag5"))
        {
            foreach (var retainer in _inventoryScanner.RetainerBag5)
            {
                if (ImGui.TreeNode("雇员背包 " + retainer.Key + "##5" + retainer.Key))
                {
                    for (int i = 0; i < retainer.Value.Length; i++)
                    {
                        var item = retainer.Value[i];
                        Utils.PrintOutObject(item, (ulong)i, new List<string>());
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("雇员已装备##retainerEquipped"))
        {
            foreach (var retainer in _inventoryScanner.RetainerEquipped)
            {
                if (ImGui.TreeNode("雇员已装备" + retainer.Key + "##equipped" + retainer.Key))
                {
                    for (int i = 0; i < retainer.Value.Length; i++)
                    {
                        var item = retainer.Value[i];
                        Utils.PrintOutObject(item, (ulong)i, new List<string>());
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("雇员市场##retainerMarket"))
        {
            foreach (var retainer in _inventoryScanner.RetainerMarket)
            {
                if (ImGui.TreeNode("雇员市场" + retainer.Key + "##market" + retainer.Key))
                {
                    for (int i = 0; i < retainer.Value.Length; i++)
                    {
                        var item = retainer.Value[i];
                        Utils.PrintOutObject(item, (ulong)i, new List<string>());
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("雇员市场价格##retainerMarketPrices"))
        {
            foreach (var retainer in _inventoryScanner.RetainerMarketPrices)
            {
                if (ImGui.TreeNode("雇员市场" + retainer.Key + "##market" + retainer.Key))
                {
                    for (int i = 0; i < retainer.Value.Length; i++)
                    {
                        var item = retainer.Value[i];
                        Utils.PrintOutObject(item, (ulong)i, new List<string>());
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("雇员水晶##retainerCrystals"))
        {
            foreach (var retainer in _inventoryScanner.RetainerCrystals)
            {
                if (ImGui.TreeNode("雇员水晶" + retainer.Key + "##crystals" + retainer.Key))
                {
                    for (int i = 0; i < retainer.Value.Length; i++)
                    {
                        var item = retainer.Value[i];
                        Utils.PrintOutObject(item, (ulong)i, new List<string>());
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }

        if (ImGui.TreeNode("雇员金币##retainerGil"))
        {
            foreach (var retainer in _inventoryScanner.RetainerGil)
            {
                if (ImGui.TreeNode("雇员金币" + retainer.Key + "##gil" + retainer.Key))
                {
                    for (int i = 0; i < retainer.Value.Length; i++)
                    {
                        var item = retainer.Value[i];
                        Utils.PrintOutObject(item, (ulong)i, new List<string>());
                    }

                    ImGui.TreePop();
                }
            }

            ImGui.TreePop();
        }
        if (ImGui.TreeNode("套装##gearsets"))
        {
            foreach (var gearSet in _inventoryScanner.GetGearSets())
            {
                ImGui.Text(gearSet.Key + ":");
                foreach (var actualset in gearSet.Value)
                {
                    ImGui.Text(actualset.Item1 + " : " + actualset.Item2);
                }
            }

            ImGui.TreePop();
        }
        var bags = new[]
        {
            InventoryType.HousingInteriorPlacedItems1,
            InventoryType.HousingInteriorPlacedItems2,
            InventoryType.HousingInteriorPlacedItems3,
            InventoryType.HousingInteriorPlacedItems4,
            InventoryType.HousingInteriorPlacedItems5,
            InventoryType.HousingInteriorPlacedItems6,
            InventoryType.HousingInteriorPlacedItems7,
            InventoryType.HousingInteriorPlacedItems8,
            InventoryType.HousingInteriorPlacedItems9,
            InventoryType.HousingInteriorPlacedItems10,
            InventoryType.HousingInteriorPlacedItems11,
            InventoryType.HousingInteriorPlacedItems12,
            InventoryType.HousingInteriorStoreroom1,
            InventoryType.HousingInteriorStoreroom2,
            InventoryType.HousingInteriorStoreroom3,
            InventoryType.HousingInteriorStoreroom4,
            InventoryType.HousingInteriorStoreroom5,
            InventoryType.HousingInteriorStoreroom6,
            InventoryType.HousingInteriorStoreroom7,
            InventoryType.HousingInteriorStoreroom8,
            InventoryType.HousingInteriorStoreroom9,
            InventoryType.HousingInteriorStoreroom10,
            InventoryType.HousingInteriorStoreroom11,
            InventoryType.HousingExteriorAppearance,
            InventoryType.HousingInteriorAppearance,
            InventoryType.HousingExteriorPlacedItems,
            InventoryType.HousingExteriorPlacedItems2,
            InventoryType.HousingExteriorStoreroom,
            InventoryType.HousingExteriorStoreroom2,
        };

        if (ImGui.TreeNode("房屋背包"))
        {
            foreach (var bagType in bags)
            {
                var bag = _inventoryScanner.GetInventoryByType(bagType);
                var bagLoaded = _inventoryScanner.IsBagLoaded(bagType);
                if (ImGui.TreeNode(bagType.ToString() + (bagLoaded ? " (Loaded)" : " (Not Loaded)")))
                {
                    var itemCount = bag.Count(c => c.ItemId != 0);
                    ImGui.Text(itemCount + "/" + bag.Length);
                    for (int i = 0; i < bag.Length; i++)
                    {
                        var item = bag[i];
                        Utils.PrintOutObject(item, (ulong)i, new List<string>());
                    }

                    ImGui.TreePop();
                }

            }
            ImGui.TreePop();
        }
    }
}