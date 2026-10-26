using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.WizardSettings;

public class CraftNotificationsReportToChatSetting : GenericBooleanSetting
{
    private bool _shouldAdd;

    public CraftNotificationsReportToChatSetting(ILogger<CraftNotificationsReportToChatSetting> logger,
        ImGuiService imGuiService) : base("CraftNotificationsReportToChat",
        "将获取报告到聊天频道",
        "获取属于制作列表的物品时，将进度输出到聊天频道。制作列表必须启用才会通知。",
        true,
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