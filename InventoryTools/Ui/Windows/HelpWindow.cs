using System.Numerics;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Extensions;
using InventoryTools.Logic;
using Dalamud.Interface.Utility.Raii;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Ui
{
    public class HelpWindow : GenericWindow
    {
        private readonly InventoryToolsConfiguration _configuration;

        public HelpWindow(ILogger<HelpWindow> logger, MediatorService mediator, ImGuiService imGuiService, InventoryToolsConfiguration configuration, string name = "Help Window") : base(logger, mediator, imGuiService, configuration, name)
        {
            _configuration = configuration;
        }
        public override void Initialize()
        {
            WindowName = "帮助";
            Key = "help";
        }

        public override bool SaveState => false;
        public override Vector2? DefaultSize { get; } = new Vector2(700, 700);
        public override  Vector2? MaxSize { get; } = new Vector2(2000, 2000);
        public override  Vector2? MinSize { get; } = new Vector2(200, 200);
        public override string GenericKey { get; } = "help";
        public override string GenericName { get; } = "帮助";
        public override bool DestroyOnClose => true;


        public override void DrawWindow()
        {
            using (var sideBarChild =
                   ImRaii.Child("SideBar", new Vector2(150, -1) * ImGui.GetIO().FontGlobalScale, true))
            {
                if (sideBarChild.Success)
                {
                    if (ImGui.Selectable("1. 常规", _configuration.SelectedHelpPage == 0))
                    {
                        _configuration.SelectedHelpPage = 0;
                    }

                    if (ImGui.Selectable("2. 筛选基础", _configuration.SelectedHelpPage == 1))
                    {
                        _configuration.SelectedHelpPage = 1;
                    }

                    if (ImGui.Selectable("3. 筛选", _configuration.SelectedHelpPage == 2))
                    {
                        _configuration.SelectedHelpPage = 2;
                    }

                    if (ImGui.Selectable("4. 关于", _configuration.SelectedHelpPage == 3))
                    {
                        _configuration.SelectedHelpPage = 3;
                    }
                }
            }

            ImGui.SameLine();

            using (var mainChild = ImRaii.Child("###ivHelpView", new Vector2(-1, -1), true))
            {
                if (mainChild.Success)
                {
                    if (_configuration.SelectedHelpPage == 0)
                    {
                        ImGui.TextWrapped(
                            "亚拉戈工具箱是一个多功能插件，提供三大主要功能：追踪/显示你的背包数据、帮助你规划制作、提供物品信息。还有其他功能，详见功能。");
                        ImGui.TextWrapped(
                            "如果你用过 Teamcraft 或 Garland Tools，它从两者中汲取了一些灵感。");
                        ImGui.NewLine();
                        ImGui.TextUnformatted("背包追踪：");
                        ImGui.Separator();
                        ImGui.TextWrapped("插件会尽力追踪你的背包。部分背包仅在首次访问时缓存。如果你没有看到雇员/部队/幻化柜等，请先查看它们，否则插件无法缓存。");
                        ImGui.TextWrapped("插件了解物品后，你可以创建列表来缩小特定物品的搜索范围、帮助你整理物品以及完成许多其他操作。");
                        ImGui.NewLine();

                        ImGui.TextUnformatted("制作规划：");
                        ImGui.Separator();
                        ImGui.TextWrapped("插件有专门的制作窗口，让你创建想要制作的物品列表。它会制定计划，将每件物品分解为其组成部分，并告诉你缺少什么。它会告诉你所需物品的位置，如果缺少任何物品，会指引你前往寻找/购买缺失物品的地点。");
                        ImGui.TextWrapped("如果你用过 Teamcraft，你会很快上手。");
                        ImGui.NewLine();

                        ImGui.TextUnformatted("物品信息：");
                        ImGui.Separator();
                        ImGui.TextWrapped("插件拥有相当全面的每件物品信息数据库。如果你用过 Garland Tools，提供的信息非常相似。点击插件内物品的图标将始终打开该物品的信息窗口。");
                        ImGui.NewLine();

                        ImGui.TextUnformatted("高亮：");
                        ImGui.Separator();
                        ImGui.TextWrapped("使用物品列表或制作列表时，你可以切换高亮。这会在游戏内高亮物品，让你清楚看到物品的位置。当插件窗口激活时，可勾选高亮复选框启用该列表的高亮。若想用宏触发此功能，请查看帮助的命令部分，你可以切换后台高亮。");
                        ImGui.NewLine();

                        ImGui.TextUnformatted("这是非常基础的指南，更多信息请查看 Wiki。");
                        if (ImGui.Button("打开 Wiki"))
                        {
                            "https://github.com/Critical-Impact/InventoryTools/wiki/1.-Overview".OpenBrowser();
                        }
                    }
                    else if (_configuration.SelectedHelpPage == 1)
                    {
                        ImGui.PushTextWrapPos();
                        ImGui.Text("列表是插件让你查看所需或尝试整理的物品的核心方式。");
                        ImGui.Text("目前可以创建 3 种类型的列表。");
                        ImGui.PopTextWrapPos();
                        ImGui.NewLine();

                        ImGui.Text("搜索列表");
                        ImGui.Separator();
                        ImGui.PushTextWrapPos();

                        ImGui.TextUnformatted("此类列表允许你在所有背包中搜索特定物品。如果你只需要查找物品而不需要整理帮助，这就是你需要的列表类型。");
                        ImGui.TextUnformatted("使用示例：");
                        ImGui.BulletText("查找制作所需的素材。");
                        ImGui.BulletText("查找你放置在某处的房屋物品。");
                        ImGui.BulletText("查看刚拾取物品的价值。");
                        ImGui.BulletText("查看特定物品是否已在幻化柜或收藏柜中。");
                        ImGui.BulletText("无需前往传唤铃即可检查雇员的装备。");
                        ImGui.BulletText("检查你拥有的物品是否可放入收藏柜。");
                        ImGui.PopTextWrapPos();
                        ImGui.NewLine();

                        ImGui.Text("排序筛选");
                        ImGui.Separator();
                        ImGui.PushTextWrapPos();
                        ImGui.TextUnformatted("此类列表以搜索列表为基础，但还可以让你选择物品的整理目标。它会尝试向你展示将物品存入所选目标的最优方案。");
                        ImGui.TextUnformatted("使用示例：");
                        ImGui.BulletText("制作后将素材收好并避免重复。");
                        ImGui.BulletText("将超过一定品级的物品存入陆行鸟鞍囊备用。");
                        ImGui.BulletText("查找部队储物柜中独有的物品并将其放入其中。");
                        ImGui.PopTextWrapPos();

                        ImGui.NewLine();
                        ImGui.Text("游戏物品筛选");
                        ImGui.Separator();
                        ImGui.PushTextWrapPos();
                        ImGui.TextUnformatted("此筛选器允许你在游戏物品目录中的所有物品中搜索。");
                        ImGui.TextUnformatted("使用示例：");
                        ImGui.BulletText("搜索幻化");
                        ImGui.BulletText("查看你尚未获得的坐骑/宠物");
                        ImGui.BulletText("追踪游戏内所有物品的价格");
                        ImGui.PopTextWrapPos();
                    }
                    else if (_configuration.SelectedHelpPage == 2)
                    {
                        ImGui.TextUnformatted("高级搜索/筛选语法：");
                        ImGui.Separator();
                        ImGui.TextWrapped(
                            "创建列表或搜索列表结果时，可以使用一系列运算符让搜索更精确。可用运算符取决于搜索对象，目前支持 !、<、>、>=、<=、=。");
                        ImGui.TextWrapped(
                            "! - 显示不包含输入内容的全部结果 - 适用于文本和数字。");
                        ImGui.TextWrapped(
                            "< - 显示值小于输入内容的全部结果 - 适用于数字。");
                        ImGui.TextWrapped(
                            "> - 显示值大于输入内容的全部结果 - 适用于数字。");
                        ImGui.TextWrapped(
                            ">= - 显示值大于或等于输入内容的全部结果 - 适用于数字。");
                        ImGui.TextWrapped(
                            "<= - 显示值小于或等于输入内容的全部结果 - 适用于数字。");
                        ImGui.TextWrapped(
                            "= - 显示值与输入内容完全相等的全部结果 - 适用于文本和数字。");
                        ImGui.TextWrapped(
                            "&& 和 || 分别表示与和或 - 可用于串联运算符。");
                    }
                    else if (_configuration.SelectedHelpPage == 3)
                    {
                        ImGui.TextUnformatted("关于：");
                        ImGui.TextUnformatted(
                            "这个插件是我利用业余时间开发的，是出于热爱的作品，我希望能在今后一段时间内持续发布更新。");
                        ImGui.TextUnformatted(
                            "如果遇到任何问题，请通过插件安装器的反馈按钮提交反馈。");
                        ImGui.TextUnformatted("插件 Wiki：");
                        ImGui.SameLine();
                        if (ImGui.Button("打开##WikiBtn"))
                        {
                            "https://github.com/Critical-Impact/InventoryTools/wiki/1.-Overview".OpenBrowser();
                        }

                        ImGui.TextUnformatted("发现 BUG？");
                        ImGui.SameLine();
                        if (ImGui.Button("打开##BugBtn"))
                        {
                            "https://github.com/Critical-Impact/InventoryTools/issues".OpenBrowser();
                        }
                    }
                }
            }
        }

        public override FilterConfiguration? SelectedConfiguration => null;

        public override void Invalidate()
        {

        }
    }
}