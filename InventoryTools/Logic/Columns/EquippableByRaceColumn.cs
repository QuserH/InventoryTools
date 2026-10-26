using System.Collections.Generic;
using AllaganLib.GameSheets.Model;
using CriticalCommonLib.Enums;
using CriticalCommonLib.Extensions;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class EquippableByRaceColumn : TextColumn
    {
        public EquippableByRaceColumn(ILogger<EquippableByRaceColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            return searchResult.Item.EquipRace.FormattedName();
        }

        public override string Name { get; set; } = "可装备种族";
        public override float Width { get; set; } = 200;
        public override string HelpText { get; set; } = "显示物品是否可被特定种族装备。";
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}