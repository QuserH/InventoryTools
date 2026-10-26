using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class AcquisitionTrackerPersistStateSetting : GenericIntegerSetting
{
    public AcquisitionTrackerPersistStateSetting(ILogger<AcquisitionTrackerPersistStateSetting> logger, ImGuiService imGuiService) : base("AcquisitionTrackerPersistState", "状态保留时间（秒）", "获取追踪器会记录你的当前状态（制作、采集等）。状态变回无操作后默认保持 2 秒，用于捕捉追踪器延迟。如果追踪器未能记录你制作/采集的物品，请调高此数值。", 2, SettingCategory.Troubleshooting, SettingSubCategory.AcquisitionTracker,"1.12.0.8", logger, imGuiService)
    {
    }
}