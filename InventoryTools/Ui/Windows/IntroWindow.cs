using System.Numerics;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic;
using Dalamud.Interface.Utility.Raii;
using InventoryTools.Mediator;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Ui
{
    public class IntroWindow : GenericWindow
    {
        public IntroWindow(ILogger<IntroWindow> logger, MediatorService mediator, ImGuiService imGuiService, InventoryToolsConfiguration configuration, string name = "Intro Window") : base(logger, mediator, imGuiService, configuration, name)
        {
        }
        public override void Initialize()
        {
            WindowName = "亚拉戈工具箱";
            Flags =
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar;
            Key = "intro";
        }


        public override void Invalidate()
        {
        }

        public override FilterConfiguration? SelectedConfiguration => null;
        public override string GenericKey { get; } = "intro";
        public override string GenericName { get; } = "介绍";
        public override bool DestroyOnClose => true;

        public override void DrawWindow()
        {
            using (var leftChild = ImRaii.Child("Left", new Vector2(200, 0)))
            {
                if (leftChild.Success)
                {
                    ImGui.SetCursorPosY(40);
                    ImGui.Image(ImGuiService.GetImageTexture("icon-hor").Handle, new Vector2(200, 200) * ImGui.GetIO().FontGlobalScale);
                }
            }
            ImGui.SameLine();
            using (var rightChild = ImRaii.Child("Right", new Vector2(0, 0), false, ImGuiWindowFlags.NoScrollbar))
            {
                if (rightChild.Success)
                {
                    using (var textChild = ImRaii.Child("Text", new Vector2(0, -32)))
                    {
                        if (textChild.Success)
                        {
                            ImGui.TextWrapped("欢迎使用亚拉戈工具箱。");
                            ImGui.TextWrapped(
                                "亚拉戈工具箱是《最终幻想XIV》的一个插件，提供以下功能：");
                            using (ImRaii.PushIndent())
                            {
                                ImGui.Bullet();
                                ImGui.Text("追踪你的背包");
                                ImGui.Bullet();
                                ImGui.Text("规划你的制作");
                                ImGui.Bullet();
                                ImGui.Text("提供物品、怪物、副本等更多信息");
                            }

                            ImGui.TextWrapped(
                                "你可以使用命令快捷方式打开各种新窗口（主筛选器或从主窗口）。");
                            ImGui.TextWrapped(
                                "如果拿不准，请右键点击物品或表格行以查看更多选项！");
                            ImGui.TextWrapped(
                                "要了解不同功能，建议前往设置部分，阅读 ? 图标提供的信息。");
                        }
                    }

                    using (var buttonsChild = ImRaii.Child("Buttons", new Vector2(0, 32)))
                    {
                        if (buttonsChild.Success)
                        {
                            if (ImGui.Button("关闭"))
                            {
                                Close();
                            }

                            ImGui.SameLine(0, 4);
                            if (ImGui.Button("关闭并打开主窗口"))
                            {
                                Close();
                                MediatorService.Publish(new OpenGenericWindowMessage(typeof(FiltersWindow)));
                            }
                        }
                    }
                }
            }
        }

        public override Vector2? DefaultSize { get; } = new Vector2(800, 300);
        public override Vector2? MaxSize { get; } = new Vector2(800, 300);
        public override Vector2? MinSize { get; } = new Vector2(800, 300);
        public override bool SaveState => false;
    }
}