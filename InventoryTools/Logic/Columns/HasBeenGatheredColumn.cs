using CriticalCommonLib.Services;
using InventoryTools.Localization;

using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns;

public class HasBeenGatheredColumn : CheckboxColumn
{
    private readonly IGameInterface _gameInterface;

    public HasBeenGatheredColumn(ILogger<HasBeenGatheredColumn> logger, ImGuiService imGuiService, IGameInterface gameInterface) : base(logger, imGuiService)
    {
        _gameInterface = gameInterface;
    }
    public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

    public override bool? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        return _gameInterface.IsItemGathered(searchResult.Item.RowId);
    }

    public override string Name { get; set; } = "已记录于采集笔记？";
    public override string RenderName => "已记录？";
    public override float Width { get; set; } = 80;

    public override string HelpText { get; set; } =
        "当前角色是否至少采集过此物品一次？目前仅支持采矿工和园艺工。";

    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Boolean;
}