using InventoryTools.Localization;
﻿using System.Collections.Generic;
using System.Linq;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using Microsoft.Extensions.Logging;

namespace InventoryTools.Logic.Settings;

public class TooltipCategoryWhitelistSetting : MultipleChoiceSetting<uint>
{
    private readonly ExcelSheet<ItemUICategory> _itemUiCategorySheet;

    public TooltipCategoryWhitelistSetting(ILogger<TooltipCategoryWhitelistSetting> logger, ImGuiService imGuiService, ExcelSheet<ItemUICategory> itemUiCategorySheet) : base(logger, imGuiService)
    {
        _itemUiCategorySheet = itemUiCategorySheet;
    }

    public override List<uint> DefaultValue { get; set; } = new();
    public override List<uint> CurrentValue(InventoryToolsConfiguration configuration)
    {
        return configuration.TooltipWhitelistCategories;
    }

    public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, List<uint> newValue)
    {
        configuration.TooltipWhitelistCategories = newValue;
    }

    public override string Key { get; set; } = "TooltipWhitelistCategories";
    public override string Name { get; set; } = "悬浮提示类别白名单";
    public override string HelpText { get; set; } = "向悬浮提示添加信息时，是否将受影响的物品限制为这些类别？勾选悬浮提示类别黑名单后，此功能将反转。";
    public override SettingCategory SettingCategory { get; set; } = SettingCategory.ToolTips;
    public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.General;
    public override Dictionary<uint, string> GetChoices(InventoryToolsConfiguration configuration)
    {
        return _itemUiCategorySheet
            .ToDictionary(c => c.RowId, c => c.Name.ExtractText());
    }

    public override bool HideAlreadyPicked { get; set; } = true;
    public override string Version => "1.7.0.0";

}