using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns.Stats;

public class MagicalDamageColumn : IntegerColumn
{
    public MagicalDamageColumn(ILogger<MagicalDamageColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override ColumnCategory ColumnCategory => ColumnCategory.Stats;
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;

    public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        if (searchResult.Item.Base.DamageMag == 0)
        {
            return null;
        }
        return searchResult.Item.Base.DamageMag;
    }

    public override string Name { get; set; } = "魔法伤害";
    public override float Width { get; set; } = 80;
    public override string HelpText { get; set; } = "物品的魔法伤害";
}