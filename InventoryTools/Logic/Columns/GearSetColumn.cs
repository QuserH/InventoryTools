using System;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns
{
    public class GearSetColumn : TextColumn
    {
        public GearSetColumn(ILogger<GearSetColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override ColumnCategory ColumnCategory => ColumnCategory.Basic;

        public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
        {
            if (searchResult.InventoryItem != null)
            {
                if (searchResult.InventoryItem.GearSetNames == null)
                {
                    return "";
                }

                return String.Join(", ", searchResult.InventoryItem.GearSetNames);
            }

            return "";
        }
        
        public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Gearset Number"));
        public override string RenderName => LocalizationService.Ui("Gearsets");
        public override float Width { get; set; } = 100;
        public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Provides the gearsets that an item is part of."));
        public override bool HasFilter { get; set; } = true;
        public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;
    }
}