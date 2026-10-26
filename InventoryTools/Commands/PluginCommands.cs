using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using AllaganLib.Shared.Windows;
using CriticalCommonLib;
using CriticalCommonLib.Extensions;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using InventoryTools.Attributes;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Windows;
using InventoryTools.EquipmentSuggest;
using InventoryTools.Mediator;
using InventoryTools.Services.Interfaces;
using InventoryTools.Ui;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Commands
{
    public class PluginCommands
    {
        public ILogger<PluginCommands> Logger { get; }
        private readonly MediatorService _mediatorService;
        private readonly IChatUtilities _chatUtilities;
        private readonly ItemSheet _itemSheet;
        private readonly IListService _listService;
        private readonly IEnumerable<ICompendiumType> _compendiumTypes;

        public PluginCommands(MediatorService mediatorService, IChatUtilities chatUtilities, ItemSheet itemSheet, IListService listService, ILogger<PluginCommands> logger, IEnumerable<ICompendiumType> compendiumTypes)
        {
            Logger = logger;
            _mediatorService = mediatorService;
            _chatUtilities = chatUtilities;
            _itemSheet = itemSheet;
            _listService = listService;
            _compendiumTypes = compendiumTypes;
        }

        [Command("/allagantools")]
        [Aliases("/atools")]
        [HelpMessage("显示亚拉戈工具箱物品列表窗口。")]
        public void ShowHideInventoryToolsCommand(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(FiltersWindow)));
        }
        [Command("/duties")]
        [Aliases("/atduties")]
        [HelpMessage("显示亚拉戈工具箱副本窗口。")]
        public void ShowHideDutiesWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(DutiesWindow)));
        }
        [Command("/mobs")]
        [Aliases("/atmobs")]
        [HelpMessage("显示亚拉戈工具箱怪物窗口。")]
        public void ShowHideMobsWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(BNpcsWindow)));
        }
        [Command("/atnpcs")]
        [HelpMessage("显示亚拉戈工具箱NPC窗口。")]
        public void ShowHideNpcsWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(ENpcsWindow)));
        }

        [Command("/compendium")]
        [Aliases("/atc")]
        [HelpMessage("显示亚拉戈工具箱图鉴窗口。")]
        public void ShowCompendiumWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(CompendiumTypesWindow)));
        }

        [Command("/compendiumlist")]
        [Aliases("/atclt")]
        [HelpMessage("切换指定的图鉴列表窗口。")]
        public void ToggleCompendiumListWindow(string command, string args)
        {
            if (args == string.Empty)
            {
                var message = "请输入图鉴类型名称，可用的类型如下：\n";
                message += string.Join("\n", _compendiumTypes.Where(c => c.ShowInListing).Select(c => c.Plural));
                _chatUtilities.Print(message);
            }
            else
            {
                var name = args.ToLowerInvariant();
                var compendiumType = _compendiumTypes.FirstOrDefault(c =>
                    c.Singular.ToLowerInvariant() == name || c.Plural.ToLowerInvariant() == name);
                if (compendiumType != null)
                {
                    _mediatorService.Publish(new ToggleCompendiumListMessage(compendiumType));
                }
                else
                {
                    _chatUtilities.PrintError(args + "不是有效的图鉴类型。");
                }
            }
        }


        [Command("/athighlight")]
        [Aliases("/atf")]
        [HelpMessage("切换指定列表的高亮开/关，同时关闭其他所有高亮。")]
        public  void FilterToggleCommand(string command, string args)
        {
            Logger.LogTrace(command);
            Logger.LogTrace(args);
            if (args.Trim() == "")
            {
                _chatUtilities.PrintError("必须输入列表名称。");
            }
            else
            {
                _listService.ToggleActiveBackgroundList(args);
            }
        }

        [Command("/openlist")]
        [HelpMessage("打开/关闭显示单个列表内容的窗口。")]
        public  void OpenFilterCommand(string command, string args)
        {
            if (args.Trim() == "")
            {
                _chatUtilities.PrintError("必须输入列表名称。");
            }
            else
            {
                var list = _listService.GetListByKeyOrName(args.Trim());
                if (list != null)
                {
                    _mediatorService.Publish(new ToggleStringWindowMessage(typeof(FilterWindow), list.Key));
                }
                else
                {
                    _chatUtilities.PrintError("找不到该名称的列表。");
                }
            }
        }

        [Command("/crafts")]
        [HelpMessage("打开亚拉戈工具箱制作窗口")]
        public  void OpenCraftsWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(CraftsWindow)));
        }

        [Command("/airships")]
        [HelpMessage("打开亚拉戈工具箱飞空艇窗口")]
        public  void ToggleAirshipsWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(AirshipsWindow)));
        }

        [Command("/submarines")]
        [HelpMessage("打开亚拉戈工具箱潜水艇窗口")]
        public  void ToggleSubmarinesWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(SubmarinesWindow)));
        }

        [Command("/retainerventures")]
        [HelpMessage("打开亚拉戈工具箱雇员探险窗口")]
        public  void ToggleToggleRetainerTasksWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(RetainerTasksWindow)));
        }

        [Command("/atconfig")]
        [HelpMessage("打开亚拉戈工具箱设置窗口")]
        public  void OpenConfigurationWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(ConfigurationWindow)));
        }

        [Command("/athelp")]
        [HelpMessage("打开亚拉戈工具箱帮助窗口")]
        public void OpenHelpWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(HelpWindow)));
        }

        [Command("/atdebug")]
        [HelpMessage("打开亚拉戈工具箱调试窗口")]
        public void ToggleDebugWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleDalamudWindowMessage(typeof(AllaganDebugWindow)));
        }

        [Command("/atclearhighlights", "/atclearfilter")]
        [HelpMessage("清除当前活动的高亮。传入 background 或 ui 可分别关闭背景高亮和界面高亮。")]
        public void ClearFilter(string command, string args)
        {
            args = args.Trim();
            if (args == "")
            {
                _listService.ClearActiveBackgroundList();
                _listService.ClearActiveUiList();
            }
            else if (args == "background")
            {
                _listService.ClearActiveBackgroundList();
            }
            else if (args == "ui")
            {
                _listService.ClearActiveUiList();
            }
        }

        [Command("/atcloselists", "/atclosefilters")]
        [HelpMessage("关闭所有列表窗口。")]
        public void CloseFilterWindows(string command, string args)
        {
            _mediatorService.Publish(new CloseWindowsByTypeMessage(typeof(FilterWindow)));
        }

        [Command("/atclearall")]
        [HelpMessage("关闭所有列表窗口并清除所有活动高亮。传入 background 或 ui 可分别只关闭背景高亮或界面高亮。")]
        public void ClearAll(string command, string args)
        {
            ClearFilter(command, args);
            CloseFilterWindows(command,args);
        }

        [Command("/craftoverlay")]
        [HelpMessage("切换制作悬浮窗。")]
        public void CraftOverlay(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(CraftOverlayWindow)));
        }

        [Command("/atrecommend", "/atr")]
        [HelpMessage("切换装备推荐窗口。")]
        public void EquipmentRecommendation(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(EquipmentSuggestWindow)));
        }

        [Command("/chocobocolour")]
        [Aliases("/atcc")]
        [HelpMessage("切换陆行鸟染色计算器窗口。")]
        public void ToggleChocoboColourWindow(string command, string args)
        {
            _mediatorService.Publish(new ToggleGenericWindowMessage(typeof(ChocoboColourWindow)));
        }

        [Command("/moreinfo")]
        [Aliases("/itemwindow")]
        [HelpMessage("打开指定物品的详细信息窗口。可提供物品名称或物品ID。")]
        public void MoreInformation(string command, string args)
        {
            args = args.Trim();
            if(args == "")
            {
                return;
            }

            ItemRow? item = null;
            if (UInt32.TryParse(args, out uint itemId))
            {
                item = _itemSheet.GetRowOrDefault(itemId);
            }
            else
            {
                if (_itemSheet.ItemsBySearchString.TryGetValue(args.ToParseable(), out itemId))
                {
                    item = _itemSheet.GetRowOrDefault(itemId);
                }
            }
            if (item != null && item.RowId != 0)
            {
                _mediatorService.Publish(new OpenUintWindowMessage(typeof(ItemWindow), item.RowId));
            }
            else
            {
                _chatUtilities.PrintError("物品 " + args + "未找到。");
            }
        }


    }
}