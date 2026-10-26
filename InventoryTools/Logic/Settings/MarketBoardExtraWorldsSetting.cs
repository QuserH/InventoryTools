using System.Collections.Generic;
using System.Linq;
using InventoryTools.Extensions;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class MarketBoardExtraWorldsSetting : MultipleChoiceSetting<uint>
{
    public MarketBoardExtraWorldsSetting(ILogger<MarketBoardExtraWorldsSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override List<uint> DefaultValue { get; set; } = new List<uint>();
    public override List<uint> CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.MarketBoardWorldIds;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, List<uint> newValue)
    {
        configuration.MarketBoardWorldIds = newValue;
    }

    public override string Key { get; set; } = "MarketBoardExtraWorlds";
    public override string Name { get; set; } = "价格服务器";
    public override string HelpText { get; set; } = "应自动定价的额外服务器列表";
    public override SettingCategory SettingCategory { get; set; } = SettingCategory.MarketBoard;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.Market;
    public override string Version { get; } = "1.7.0.0";
    private Dictionary<uint, string>? _worldNames;
    public override Dictionary<uint, string> GetChoices(InventoryToolsConfiguration configuration)
    {
        if (_worldNames == null)
        {
            _worldNames = CnWorlds.Names.ToDictionary(c => c.Key, c => $"{c.Value.DataCenter} · {c.Value.World}");
        }

        return _worldNames;
    }

    public override bool HideAlreadyPicked { get; set; } = true;
}