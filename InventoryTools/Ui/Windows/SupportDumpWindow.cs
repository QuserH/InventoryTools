using System.Linq;
using System.Numerics;
using CriticalCommonLib.Services;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using DalaMock.Shared.Interfaces;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Ui
{
    public class SupportDumpWindow : GenericWindow
    {
        private readonly SupportDumpService _supportDumpService;
        private readonly IFileDialogManager _fileDialogManager;
        private readonly IChatUtilities _chatUtilities;

        public SupportDumpWindow(ILogger<SupportDumpWindow> logger, MediatorService mediator, ImGuiService imGuiService, InventoryToolsConfiguration configuration, SupportDumpService supportDumpService, IFileDialogManager fileDialogManager, IChatUtilities chatUtilities, string name = "Generate Support Dump") : base(logger, mediator, imGuiService, configuration, name)
        {
            _supportDumpService = supportDumpService;
            _fileDialogManager = fileDialogManager;
            _chatUtilities = chatUtilities;
        }

        public override void Initialize()
        {
            WindowName = "生成支持信息包";
            Key = "supportdump";
        }

        public override bool SaveState => false;
        public override Vector2? DefaultSize { get; } = new Vector2(500, 200);
        public override Vector2? MaxSize { get; } = new Vector2(800, 400);
        public override Vector2? MinSize { get; } = new Vector2(300, 150);
        public override string GenericKey { get; } = "supportdump";
        public override string GenericName { get; } = "生成支持信息包";
        public override bool DestroyOnClose => true;

        public override void DrawWindow()
        {
            ImGui.PushTextWrapPos();
            ImGui.TextWrapped(
                "仅在收到指示时才按此按钮，它将生成包含你的背包、亚拉戈工具箱配置和日志的 zip 文件。如果你想提供此信息以帮助修复 BUG，请在文件中启用详细日志，复现 BUG，然后生成此信息包。继续？");
            ImGui.PopTextWrapPos();
            ImGui.NewLine();

            if (ImGui.Button("继续"))
            {
                _fileDialogManager.SaveFileDialog("保存支持信息包", "*.zip", "support_dump.zip", ".zip",
                    (success, path) =>
                    {
                        if (success)
                        {
                            GenerateDump(path);
                        }
                    }, null, true);
            }

            ImGui.SameLine();

            if (ImGui.Button("取消"))
            {
                this.IsOpen = false;
            }
        }

        private void GenerateDump(string path)
        {
            var result = _supportDumpService.GenerateDump(path);
            if (result.Success)
            {
                var included = result.IncludedFiles.Any() ? string.Join(", ", result.IncludedFiles) : "无文件";
                _chatUtilities.Print(string.Format("支持信息包已保存至 {0}（{1}）。", result.ZipPath, included));
                if (result.MissingFiles.Any())
                {
                    _chatUtilities.PrintError(string.Format("以下文件未能包含在支持信息包中：{0}。",
                        string.Join(", ", result.MissingFiles)));
                }
            }
            else
            {
                _chatUtilities.PrintError("生成支持信息包失败。请查看 Dalamud 日志了解详情。");
            }

            this.IsOpen = false;
        }

        public override FilterConfiguration? SelectedConfiguration => null;

        public override void Invalidate()
        {

        }
    }
}
