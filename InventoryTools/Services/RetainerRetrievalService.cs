using System.Collections.Generic;
using CriticalCommonLib.Crafting;

namespace InventoryTools.Services;

public interface IRetainerRetrievalService
{
    bool IsRunning { get; }
    string Status { get; }
    RetainerRetrievalPlan BuildPlan(CraftList craftList, IReadOnlyDictionary<RetainerRetrievalItemKey, uint>? requestedQuantities = null);
    bool Start(CraftList craftList, IReadOnlyDictionary<RetainerRetrievalItemKey, uint>? requestedQuantities = null);
    void Cancel();
}

/// <summary>
/// Coordinates the cache plan with the guarded game-thread automation state machine.
/// </summary>
public sealed class RetainerRetrievalService : IRetainerRetrievalService
{
    private readonly RetainerRetrievalPlanner _planner;
    private readonly RetainerRetrievalAutomation _automation;

    public RetainerRetrievalService(RetainerRetrievalPlanner planner, RetainerRetrievalAutomation automation)
    {
        _planner = planner;
        _automation = automation;
    }

    public bool IsRunning => _automation.IsRunning;
    public string Status => _automation.Status;

    public RetainerRetrievalPlan BuildPlan(CraftList craftList,
        IReadOnlyDictionary<RetainerRetrievalItemKey, uint>? requestedQuantities = null) =>
        _planner.Build(craftList, requestedQuantities);

    public bool Start(CraftList craftList,
        IReadOnlyDictionary<RetainerRetrievalItemKey, uint>? requestedQuantities = null)
    {
        if (IsRunning)
            return false;

        return _automation.Start(craftList, requestedQuantities);
    }

    public void Cancel()
    {
        _automation.Cancel();
    }
}
