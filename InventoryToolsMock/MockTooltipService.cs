using CriticalCommonLib.Services;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace InventoryToolsMock;

public class MockTooltipService : ITooltipService
{
    public void AddTooltipTweak(TooltipService.TooltipTweak tooltipTweak)
    {
        
    }

    public void ActionHoveredDetour(ulong a1, int a2, uint a3, int a4, byte a5)
    {
    }

    public unsafe nint ActionTooltipDetour(AtkUnitBase* addon, void* a2, ulong a3)
    {
        return 0;
    }

    public unsafe bool ItemHoveredDetour(AgentItemDetail* a1, InventoryItem** a2, InventoryType* containerid, short* slotid, uint a5, uint slotidint, InventoryItem* a7)
    {
        return false;
    }

    public void Disable()
    {
    }

    public void Dispose()
    {
    }

    public unsafe void GenerateItemTooltipDetour(AddonItemDetail* addonItemDetail, NumberArrayData* numberArrayData,
        StringArrayData* stringArrayData)
    {
    }

    public unsafe void* GenerateActionTooltipDetour(AtkUnitBase* addonItemDetail, NumberArrayData* numberArrayData,
        StringArrayData* stringArrayData)
    {
        return (void*)0;
    }
}