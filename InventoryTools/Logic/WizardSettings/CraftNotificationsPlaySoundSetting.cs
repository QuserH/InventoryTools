using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.WizardSettings;

public class CraftNotificationsPlaySoundSetting : GenericBooleanSetting
{
    private bool _shouldAdd;

    public CraftNotificationsPlaySoundSetting(ILogger<CraftNotificationsPlaySoundSetting> logger,
        ImGuiService imGuiService) : base("CraftNotificationsPlaySound",
        "物品完成时播放音效",
        "制作列表中的物品达到所需数量时播放音效。制作列表必须启用才会通知。",
        false,
        SettingCategory.None,
        SettingSubCategory.None,
        "15.0.8",
        logger,
        imGuiService)
    {
    }

    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return _shouldAdd;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        _shouldAdd = newValue;
    }
}