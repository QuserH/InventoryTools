using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class MarketBoardUseHomeWorldSetting : BooleanSetting
{
    public override bool DefaultValue { get; set; } = true;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.MarketBoardUseHomeWorld;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.MarketBoardUseHomeWorld = newValue;
    }

    public override string Key { get; set; } = "MarketBoardUseHomeWorld";
    public override string Name { get; set; } = "使用所属服务器价格？";
    public override string HelpText { get; set; } = "是否自动为角色的所属服务器定价？";
    public override SettingCategory SettingCategory { get; set; } = SettingCategory.MarketBoard;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.Market;
    public override string Version { get; } = "1.7.0.0";

    public MarketBoardUseHomeWorldSetting(ILogger<MarketBoardUseHomeWorldSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }
}