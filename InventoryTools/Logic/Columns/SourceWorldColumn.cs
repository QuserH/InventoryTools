using CriticalCommonLib.Services;
using InventoryTools.Localization;

using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns;

public class SourceWorldColumn : TextColumn
{
    private readonly ICharacterMonitor _characterMonitor;

    public SourceWorldColumn(ILogger<SourceWorldColumn> logger, ImGuiService imGuiService, ICharacterMonitor characterMonitor) : base(logger, imGuiService)
    {
        _characterMonitor = characterMonitor;
    }
    public override ColumnCategory ColumnCategory => ColumnCategory.Inventory;
    public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        if (searchResult.InventoryItem != null)
        {
            var character = _characterMonitor.GetCharacterById(searchResult.InventoryItem.RetainerId);
            return character != null ? character.World?.Name.ExtractText() ?? "" : "";
        }

        return "";
    }

    public override string Name { get; set; } = "来源服务器";
    public override float Width { get; set; } = 120;

    public override string HelpText { get; set; } =
        "物品存放的服务器（角色、雇员或部队）。";

    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
}