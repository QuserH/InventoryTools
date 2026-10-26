using System.Collections.Generic;
using System.Linq;
using CriticalCommonLib.Services;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipDisplayUnlockCharacterSetting : MultipleChoiceSetting<ulong>
{
    private readonly ICharacterMonitor _characterMonitor;

    public TooltipDisplayUnlockCharacterSetting(ICharacterMonitor characterMonitor, ILogger<TooltipDisplayUnlockCharacterSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
        _characterMonitor = characterMonitor;
    }

    public override List<ulong> DefaultValue { get; set; } = new();
    public override List<ulong> CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipDisplayUnlockCharacters ?? DefaultValue;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, List<ulong> newValue)
    {
        configuration.TooltipDisplayUnlockCharacters = newValue;
    }

    public override string Key { get; set; } = "TooltipDisplayUnlockCharacter";
    public override string Name { get; set; } = "添加解锁状态（角色）";
    public override string HelpText { get; set; } = "显示物品解锁状态时，将显示这些角色。留空则显示所有角色。";
    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.ItemUnlockStatus;
    public override string Version { get; } = "1.11.0.4";
    public override Dictionary<ulong, string> GetChoices(InventoryToolsConfiguration configuration)
    {
        return _characterMonitor.GetPlayerCharacters().ToDictionary(c => c.Key, c => c.Value.FormattedName);
    }

    public override string GetPreviewValue(List<ulong> items)
    {
        if (items.Count == 0)
        {
            return "将显示全部角色。";
        }
        else
        {
            return string.Format("将显示 {0} 个角色。", items.Count);
        }
    }

    public override bool HideAlreadyPicked { get; set; } = false;
}
