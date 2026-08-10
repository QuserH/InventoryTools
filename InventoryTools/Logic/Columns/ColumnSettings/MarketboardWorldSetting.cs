using System.Collections.Generic;
using System.Linq;
using CriticalCommonLib.Models;
using Dalamud.Plugin.Services;
using InventoryTools.Extensions;
using InventoryTools.Logic.Columns.Abstract.ColumnSettings;
using InventoryTools.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns.ColumnSettings;

public class MarketboardWorldSetting : ChoiceColumnSetting<(uint,string)?>
{
    private readonly ExcelSheet<World> _worldSheet;
    public override string EmptyText => LocalizationService.Ui("Home World");

    public MarketboardWorldSetting(ILogger<MarketboardWorldSetting> logger, ImGuiService imGuiService, ExcelSheet<World> worldSheet) : base(logger, imGuiService)
    {
        _worldSheet = worldSheet;
    }
    public override (uint,string)? CurrentValue(ColumnConfiguration configuration)
    {
        configuration.GetSetting(Key, out uint? value);
        if (value == null)
        {
            return null;
        }

        if (value.Value == 0)
        {
            return (0, LocalizationService.Ui("Active World"));
        }

        var world = _worldSheet.GetRowOrDefault(value.Value);
        if (world == null)
        {
            return null;
        }
        return (world.Value.RowId, CnWorlds.Names.TryGetValue(world.Value.RowId, out var cnWorld) ? cnWorld.World : world.Value.Name.ExtractText());
    }

    public uint SelectedWorldId(ColumnConfiguration configuration, Character character)
    {
        var settingValue = CurrentValue(configuration);
        var selectedWorld = character.WorldId;
        if (settingValue != null)
        {
            if (settingValue.Value.Item1 == 0)
            {
                selectedWorld = character.ActiveWorldId;
            }
            else
            {
                selectedWorld = settingValue.Value.Item1;
            }
        }

        return selectedWorld;
    }

    public override void ResetFilter(ColumnConfiguration configuration)
    {
        configuration.SetSetting(Key, (uint?)null);
    }

    public override void UpdateColumnConfiguration(ColumnConfiguration configuration, (uint,string)? newValue)
    {
        configuration.SetSetting(Key, newValue?.Item1 ?? null);
    }

    public override string Key { get; set; } = "MBWorld";
    public override string Name { get; set; } = LocalizationService.Ui("World");
    public override string HelpText { get; set; } = LocalizationService.Ui(LocalizationService.Ui("The world for this column to display?"));
    public override (uint,string)? DefaultValue { get; set; } = null;
    public override List<(uint,string)?> GetChoices(ColumnConfiguration configuration)
    {
        List<(uint RowId, string FormattedName)?> worlds = CnWorlds.Names.Select(c => ((uint, string)?)(c.Key, $"{c.Value.DataCenter} · {c.Value.World}")).ToList();
        worlds.Insert(0,(0,LocalizationService.Ui("Active World")));
        return worlds;
    }

    public override string GetFormattedChoice(ColumnConfiguration filterConfiguration, (uint,string)? choice)
    {
        return choice?.Item2 ?? LocalizationService.Ui("Active World");
    }
}