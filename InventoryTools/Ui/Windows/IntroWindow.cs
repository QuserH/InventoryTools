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
            WindowName = LocalizationService.Ui(LocalizationService.Ui("Allagan Tools"));
            Flags =
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar;
            Key = "intro";
        }


        public override void Invalidate()
        {
        }

        public override FilterConfiguration? SelectedConfiguration => null;
        public override string GenericKey { get; } = "intro";
        public override string GenericName { get; } = LocalizationService.Ui("Intro");
        public override bool DestroyOnClose => true;

        public override void DrawWindow()
        {
            using (var leftChild = ImRaii.Child("Left", new Vector2(200, 0)))
            {
                if (leftChild.Success)
                {
                    ImGui.SetCursorPosY(40);
                    ImGui.Image(ImGuiService.GetImageTexture(LocalizationService.Ui("icon-hor")).Handle, new Vector2(200, 200) * ImGui.GetIO().FontGlobalScale);
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
                            ImGui.TextWrapped(LocalizationService.Ui(LocalizationService.Ui("Welcome to Allagan Tools.")));
                            ImGui.TextWrapped(
                                LocalizationService.Ui(LocalizationService.Ui("Allagan Tools is a plugin for Final Fantasy XIV that provides the following features:")));
                            using (ImRaii.PushIndent())
                            {
                                ImGui.Bullet();
                                ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Track your inventories")));
                                ImGui.Bullet();
                                ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Plan your crafts")));
                                ImGui.Bullet();
                                ImGui.Text(LocalizationService.Ui(LocalizationService.Ui("Provide information about items, monsters, duties and much more")));
                            }

                            ImGui.TextWrapped(
                                LocalizationService.Ui(LocalizationService.Ui("You can open various new windows using command shortcuts(the main filter  or from the main window.")));
                            ImGui.TextWrapped(
                                LocalizationService.Ui(LocalizationService.Ui("If you're unsure, right-click on an item or a table row for more options!")));
                            ImGui.TextWrapped(
                                LocalizationService.Ui(LocalizationService.Ui("To learn about the different features, I recommend going to the settings section and reading the information provided by the ? icons.")));
                        }
                    }

                    using (var buttonsChild = ImRaii.Child("Buttons", new Vector2(0, 32)))
                    {
                        if (buttonsChild.Success)
                        {
                            if (ImGui.Button(LocalizationService.Ui("Close")))
                            {
                                Close();
                            }

                            ImGui.SameLine(0, 4);
                            if (ImGui.Button(LocalizationService.Ui(LocalizationService.Ui("Close & Open Main Window"))))
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