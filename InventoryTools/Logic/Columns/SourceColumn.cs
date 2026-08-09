using InventoryTools.Localization;
﻿using CriticalCommonLib.Services;

using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Columns
{
    public class SourceColumn : TextColumn
    {
        private readonly ICharacterMonitor _characterMonitor;

        public SourceColumn(ILogger<SourceColumn> logger, ImGuiService imGuiService, ICharacterMonitor characterMonitor) : base(logger, imGuiService)
        {
            _characterMonitor = characterMonitor;
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Inventory;
        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.InventoryItem != null)
            {
                // 来源只显示雇员库存；角色背包显示为空
                if (searchResult.InventoryItem.InRetainer)
                {
                    return _characterMonitor.Characters.TryGetValue(searchResult.InventoryItem.RetainerId, out var character)
                        ? character.FormattedName
                        : LocalizationService.Ui("Unknown (") + searchResult.InventoryItem.RetainerId + ")";
                }
                return string.Empty;
            }

            return null;
        }
        public override string Name { get; set; } = LocalizationService.Ui("Source");
        public override float Width { get; set; } = 100.0f;
        public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Shows the character/retainer an item is located in."));
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
        public override FilterType DefaultIn => Logic.FilterType.SearchFilter | Logic.FilterType.SortingFilter | Logic.FilterType.CraftFilter | Logic.FilterType.HistoryFilter;
    }
}