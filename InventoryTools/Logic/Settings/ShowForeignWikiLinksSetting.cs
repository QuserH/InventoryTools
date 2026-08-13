using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class ShowForeignWikiLinksSetting : BooleanSetting
{
    public ShowForeignWikiLinksSetting(ILogger<ShowForeignWikiLinksSetting> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
    }

    public override bool DefaultValue { get; set; } = false;
    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.ShowForeignWikiLinks;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        configuration.ShowForeignWikiLinks = newValue;
    }

    public override string Key { get; set; } = "ShowForeignWikiLinks";
    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Show Foreign Wiki Links?"));
    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("Should the item right-click menu show links to foreign wiki sites (Garland Tools, Teamcraft, Universalis, Gamer Escape, Console Games Wiki)?"));

    public override SettingCategory SettingCategory { get; set; } = SettingCategory.General;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override string Version => "1.7.0.0";
}
