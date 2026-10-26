using System.Numerics;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Logic.Settings.Abstract.Generic;
using InventoryTools.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings;

public class TooltipAmountOwnedColorSetting : GenericGameColorSetting
{
    public override uint? Order { get; } = 1;

    public TooltipAmountOwnedColorSetting(ILogger<TooltipAmountOwnedColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipAmountOwnedColor", "文字颜色", "启用后，添加物品位置悬浮提示的文字应为哪种颜色？", null, SettingCategory.ToolTips, SettingSubCategory.AddItemLocations, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 8;
    }
}


public class TooltipMarketPricingColorSetting : GenericGameColorSetting
{
    public override uint? Order { get; } = 1;

    public TooltipMarketPricingColorSetting(ILogger<TooltipMarketPricingColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipMarketPricingColor", "文字颜色", "启用后，市场价格悬浮提示的文字应为哪种颜色？", null, SettingCategory.ToolTips, SettingSubCategory.MarketPricing, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 559;
    }
}
public class TooltipAmountToRetrieveColorSetting : GenericGameColorSetting
{
    public override uint? Order { get; } = 1;

    public TooltipAmountToRetrieveColorSetting(ILogger<TooltipAmountToRetrieveColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipAmountToRetrieveColor", "文字颜色", "启用后，取回数量悬浮提示的文字应为哪种颜色？", null, SettingCategory.ToolTips, SettingSubCategory.AmountToRetrieve, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 37;
    }
}
public class TooltipItemUnlockStatusColorSetting : GenericGameColorSetting
{
    public override uint? Order { get; } = 1;

    public TooltipItemUnlockStatusColorSetting(ILogger<TooltipItemUnlockStatusColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipItemUnlockStatusColor", "文字颜色", "启用后，物品解锁状态悬浮提示的文字应为哪种颜色？", null, SettingCategory.ToolTips, SettingSubCategory.ItemUnlockStatus, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 555;
    }
}
public class TooltipSourceInformationColorSetting : GenericGameColorSetting
{
    public override uint? Order { get; } = 1;

    public TooltipSourceInformationColorSetting(ILogger<TooltipSourceInformationColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipSourceInformationColor", "文字颜色", "启用后，来源信息悬浮提示的文字应为哪种颜色？", null, SettingCategory.ToolTips, SettingSubCategory.SourceInformation, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 502;
    }
}
public class TooltipUseInformationColorSetting : GenericGameColorSetting
{
    public override uint? Order { get; } = 1;

    public TooltipUseInformationColorSetting(ILogger<TooltipUseInformationColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipUseInformationColor", "文字颜色", "启用后，用途信息悬浮提示的文字应为哪种颜色？", null, SettingCategory.ToolTips, SettingSubCategory.UseInformation, "1.11.0.11", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 60;
    }
}
public class TooltipIngredientPatchTooltipColorSetting : GenericGameColorSetting
{
    public override uint? Order { get; } = 1;

    public TooltipIngredientPatchTooltipColorSetting(ILogger<TooltipIngredientPatchTooltipColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipIngredientPatchTooltipColor", "文字颜色", "启用后，素材版本悬浮提示的文字应为哪种颜色？", null, SettingCategory.ToolTips, SettingSubCategory.IngredientPatch, "1.12.0.12", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 540;
    }
}
public class TooltipGlamourReadySetColorSetting : GenericGameColorSetting
{
    public override uint? Order { get; } = 1;

    public TooltipGlamourReadySetColorSetting(ILogger<TooltipGlamourReadySetColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipGlamourReadySetColor", "文字颜色", "启用后，套装幻化悬浮提示的文字应为哪种颜色？", null, SettingCategory.ToolTips, SettingSubCategory.GlamourReadySet, "1.12.0.0", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 500;
    }
}

public class TooltipGlamourReadySetAcquiredColorSetting : GenericGameColorSetting
{
    public override uint? Order { get; } = 2;

    public TooltipGlamourReadySetAcquiredColorSetting(ILogger<TooltipGlamourReadySetAcquiredColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipGlamourReadySetAcquiredColor", "已获得物品颜色", "详细模式下，已获得的物品应显示为哪种颜色？", null, SettingCategory.ToolTips, SettingSubCategory.GlamourReadySet, "1.13.0.0", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 45;
    }
}
public class TooltipGlamourReadySetNotAcquiredColorSetting : GenericGameColorSetting
{
    public override uint? Order { get; } = 3;

    public TooltipGlamourReadySetNotAcquiredColorSetting(ILogger<TooltipGlamourReadySetNotAcquiredColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipGlamourReadySetNotAcquiredColor", "未获得物品颜色", "详细模式下，尚未获得的物品应显示为哪种颜色？", null, SettingCategory.ToolTips, SettingSubCategory.GlamourReadySet, "1.13.0.0", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 17;
    }
}
public class TooltipCofferLootColorSetting : GenericGameColorSetting
{
    public override uint? Order { get; } = 1;

    public TooltipCofferLootColorSetting(ILogger<TooltipCofferLootColorSetting> logger, ImGuiService imGuiService, ExcelSheet<UIColor> uiColorSheet) : base("TooltipCofferLootColor", "文字颜色", "启用后，宝箱战利品悬浮提示的文字应为哪种颜色？", null, SettingCategory.ToolTips, SettingSubCategory.CofferLoot, "1.12.0.0", logger, imGuiService, uiColorSheet)
    {
        this.DefaultValue = 541;
    }
}