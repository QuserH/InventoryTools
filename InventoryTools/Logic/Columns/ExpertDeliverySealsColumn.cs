using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class ExpertDeliverySealsColumn : IntegerColumn
{
    public ExpertDeliverySealsColumn(ILogger<ExpertDeliverySealsColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override ColumnCategory ColumnCategory => ColumnCategory.Basic;
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;

    public override int? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        if (searchResult.Item.IsExpertDelivery)
        {
            return (int?)searchResult.Item.GetUsesByType<ItemGCExpertDeliverySource>(ItemInfoType.GCExpertDelivery).FirstOrDefault()?.SealsRewarded ?? null;
        }

        return null;
    }

    public override string Name { get; set; } = "稀有品筹备军票奖励";
    public override float Width { get; set; } = 90;

    public override string HelpText { get; set; } =
        "将此物品作为稀有品筹备交付时奖励的军票数量。";
}