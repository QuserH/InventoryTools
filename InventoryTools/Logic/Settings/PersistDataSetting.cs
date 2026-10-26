using InventoryTools.Boot;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class PersistDataSetting : GenericBooleanSetting
{
    private readonly BootConfiguration _bootConfiguration;

    public PersistDataSetting(ILogger<PersistDataSetting> logger, BootConfiguration bootConfiguration, ImGuiService imGuiService) : base("PersistData", "保留缓存数据", "亚拉戈工具箱首次启动时需要计算信息，视电脑性能可能需要 5-10 秒。启用后，该数据将在更新之间保留，加快插件启动时间。", true, SettingCategory.Troubleshooting, SettingSubCategory.General, "14.0.5", logger, imGuiService)
    {
        _bootConfiguration = bootConfiguration;
    }

    public override bool CurrentValue(InventoryToolsConfiguration configuration)
    {
        return _bootConfiguration.PersistLuminaCache;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, bool newValue)
    {
        _bootConfiguration.PersistLuminaCache = newValue;
    }
}