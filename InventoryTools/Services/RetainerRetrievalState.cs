namespace InventoryTools.Services;

public enum RetainerRetrievalState
{
    Idle,
    FindingBell,
    WaitingForRetainerList,
    SelectingRetainer,
    SelectingRetainerAction,
    WaitingForInventory,
    RetrievingItem,
    WaitingForQuantityInput,
    WaitingForInventoryChange,
    ClosingRetainerInventory,
    SelectingQuit,
    ClosingRetainerList,
    Completed,
    Failed,
    Cancelled,
}
