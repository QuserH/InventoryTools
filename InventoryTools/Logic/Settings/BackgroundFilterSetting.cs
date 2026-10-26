using System.Collections.Generic;
using System.Linq;
using InventoryTools.Logic.Settings.Abstract;
using InventoryTools.Services;
using InventoryTools.Services.Interfaces;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings
{
    public class BackgroundFilterSetting : ChoiceSetting<string>
    {
        private readonly IListService _listService;

        public BackgroundFilterSetting(ILogger<BackgroundFilterSetting> logger, ImGuiService imGuiService, IListService listService) : base(logger, imGuiService)
        {
            _listService = listService;
        }
        public override string DefaultValue { get; set; } = "";
        public override string CurrentValue(InventoryToolsConfiguration configuration)
        {
            return configuration.ActiveBackgroundFilter ?? "";
        }

        public override void UpdateFilterConfiguration(InventoryToolsConfiguration configuration, string newValue)
        {
            if (newValue == "")
            {
                _listService.ClearActiveBackgroundList();
            }
            else
            {
                _listService.SetActiveBackgroundListByKey(newValue);
            }
        }

        public override string Key { get; set; } = "BackgroundFilter";
        public override string Name { get; set; } = "后台列表高亮";

        public override string HelpText { get; set; } =
            "这是亚拉戈工具箱窗口不可见时当前高亮的列表。可通过关联的斜杠命令切换此列表。";

        public override SettingCategory SettingCategory { get; set; } = SettingCategory.Lists;
        public override SettingSubCategory SettingSubCategory { get; } = SettingSubCategory.ActiveLists;

        public override Dictionary<string, string> Choices
        {
            get
            {
                var filterItems = new Dictionary<string, string> {{"", "None"}};
                foreach (var config in _listService.Lists.Where(c => !c.CraftListDefault))
                {
                    filterItems.Add(config.Key, config.Name);
                }
                return filterItems;
            }
        }
        public override string Version => "1.7.0.0";
    }
}