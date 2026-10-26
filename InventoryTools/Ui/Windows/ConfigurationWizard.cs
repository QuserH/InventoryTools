using System.Collections.Generic;
using System.Numerics;
using CriticalCommonLib.Services.Mediator;
using DalaMock.Host.Mediator;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Logic;
using InventoryTools.Logic.Features;
using InventoryTools.Mediator;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using OtterGui.Raii;
using InventoryTools.Localization;

namespace InventoryTools.Ui;

public class ConfigurationWizard : GenericWindow
{
    private readonly ConfigurationWizardService _configurationWizardService;
    private readonly InventoryToolsConfiguration _configuration;

    public ConfigurationWizard(ILogger<ConfigurationWizard> logger, MediatorService mediator, ImGuiService imGuiService, InventoryToolsConfiguration configuration, ConfigurationWizardService configurationWizardService, string name = "Configuration Wizard") : base(logger, mediator, imGuiService, configuration, name)
    {
        _configurationWizardService = configurationWizardService;
        _configuration = configuration;
    }
    private List<IFeature> _availableFeatures = new();
    private int _currentFeature;
    public override void Initialize()
    {
        WindowName = "设置向导";
        Key = "wizard";
        _availableFeatures = _configurationWizardService.GetNewFeatures();
    }

    public override string GenericKey => "wizard";
    public override string GenericName => "设置向导";
    public override bool DestroyOnClose => true;
    public override bool SaveState => false;
    public override Vector2? DefaultSize { get; } = new(750, 500);
    public override Vector2? MaxSize { get; } = new(1000, 1000);
    public override Vector2? MinSize { get; } = new(750, 350);

    private bool CanGoPrevious => _currentFeature != 0;
    private bool CanGoNext => _availableFeatures.Count != 0 && _currentFeature != _availableFeatures.Count;

    private void NextStep()
    {
        if (_currentFeature == 0)
        {
            _currentFeature = 1;
        }
        else if(_currentFeature == _availableFeatures.Count)
        {

        }
        else
        {
            _currentFeature++;
        }
    }

    private void PreviousStep()
    {
        if (_currentFeature != 0)
        {
            _currentFeature--;
        }
    }

    public override void DrawWindow()
    {
        using (var sideBar = ImRaii.Child("sideBar", new Vector2(150, 0) * ImGui.GetIO().FontGlobalScale, true))
        {
            if (sideBar)
            {
                using (var sideBarMenu = ImRaii.Child("sideBarMenu",
                           new Vector2(150, -120) * ImGui.GetIO().FontGlobalScale, false))
                {
                    if (sideBarMenu)
                    {
                        using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.HealerGreen,
                                   _currentFeature == 0))
                        {
                            ImGui.Text("欢迎");
                        }

                        for (var index = 0; index < _availableFeatures.Count; index++)
                        {
                            var feature = _availableFeatures[index];
                            using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.HealerGreen,
                                       index + 1 == _currentFeature))
                            {
                                ImGui.Text((index + 1) + ". " + feature.Name);
                            }
                        }
                    }
                }
                using (var sideBarImage = ImRaii.Child("sideBarImage",
                           new Vector2(150, 0) * ImGui.GetIO().FontGlobalScale, false))
                {
                    if (sideBarImage)
                    {
                        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + 12.5f);
                        ImGui.Image(ImGuiService.GetImageTexture("icon").Handle, new (100,100));
                    }
                }
            }
        }
        ImGui.SameLine();
        using (var mainWindow = ImRaii.Child("mainWindow", new Vector2(0, 0)))
        {
            if (mainWindow)
            {
                using (var mainContainer = ImRaii.Child("mainContainer", new Vector2(-1, -80) * ImGui.GetIO().FontGlobalScale, true))
                {
                    if (mainContainer)
                    {
                        if (_currentFeature == 0)
                        {
                            if (_configurationWizardService.ConfiguredOnce)
                            {
                                ImGui.TextWrapped("欢迎回到亚拉戈工具箱设置向导。");
                                ImGui.Separator();
                                ImGui.TextWrapped(
                                    "有新的功能可供配置，你选择了在此情况下显示此窗口。");
                                ImGui.NewLine();
                            }
                            else
                            {
                                ImGui.TextWrapped("欢迎使用亚拉戈工具箱设置向导。");
                                ImGui.Separator();
                                ImGui.TextWrapped(
                                    "本向导将引导你完成最常用功能的设置。若你允许，新功能发布时向导会再次显示，因为功能通常由用户自行配置和启用。");
                                ImGui.NewLine();
                                ImGui.TextWrapped("如果你是第一次使用亚拉戈工具箱，建议打开帮助窗口并阅读常规部分。它为你介绍了插件的功能。");
                                ImGui.TextWrapped("如果你是老用户，可以随意关闭此窗口。");
                                if (ImGui.Button("打开帮助"))
                                {
                                    MediatorService.Publish(new ToggleGenericWindowMessage(typeof(HelpWindow)));
                                }
                                ImGui.NewLine();
                            }


                        }
                        else
                        {
                            for (var index = 0; index < _availableFeatures.Count; index++)
                            {
                                var feature = _availableFeatures[index];
                                if (_currentFeature - 1 == index)
                                {
                                    ImGui.Text(feature.Name);
                                    ImGui.Separator();
                                    ImGui.PushTextWrapPos();
                                    ImGui.Text(feature.Description);
                                    ImGui.PopTextWrapPos();
                                    ImGui.Separator();
                                    foreach (var setting in _configurationWizardService.GetApplicableSettings(feature))
                                    {
                                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                                        setting.Draw(_configuration, setting.WizardName, true, true);
                                        ImGui.SetCursorPosY(ImGui.GetCursorPosY() + 5);
                                    }
                                }
                            }
                        }
                    }
                }

                using (var nextPrevBar = ImRaii.Child("nextPrevBar", new Vector2(-1, -1) * ImGui.GetIO().FontGlobalScale, true))
                {
                    if (nextPrevBar)
                    {
                        if (_currentFeature == 0)
                        {
                            if (_configurationWizardService.ConfiguredOnce)
                            {
                                if (ImGui.Button("继续"))
                                {
                                    NextStep();
                                    _configuration.ShowWizardNewFeatures = true;
                                }

                                if (ImGui.Button("关闭（下次加载插件时再次显示）"))
                                {
                                    Close();
                                    _configuration.ShowWizardNewFeatures = true;
                                }
                            }
                            else
                            {
                                if (ImGui.Button("继续（下次有新功能时显示）"))
                                {
                                    NextStep();
                                    _configuration.ShowWizardNewFeatures = true;
                                }

                                ImGui.SameLine();
                                if (ImGui.Button("继续（不再显示向导）"))
                                {
                                    NextStep();
                                    _configuration.ShowWizardNewFeatures = false;
                                }

                                if (ImGui.Button("关闭（下次加载插件时再次显示）"))
                                {
                                    Close();
                                    _configuration.ShowWizardNewFeatures = true;
                                }

                                ImGui.SameLine();
                                if (ImGui.Button("关闭（不再显示向导）"))
                                {
                                    Close();
                                    _configuration.ShowWizardNewFeatures = false;
                                }
                            }
                        }
                        else
                        {
                            var canGoPrevious = CanGoPrevious;
                            using var disabled = ImRaii.Disabled(!canGoPrevious);

                            if (ImGui.Button("上一步"))
                            {
                                PreviousStep();
                            }

                            disabled.Dispose();

                            ImGui.SameLine();
                            var canGoNext = CanGoNext;

                            if (canGoNext && ImGui.Button("下一步"))
                            {
                                NextStep();
                            }

                            if (!canGoNext && ImGui.Button("完成"))
                            {
                                Finish();
                            }

                        }
                    }
                }
            }
        }
    }

    private void Finish()
    {
        this.Close();
        _currentFeature = 0;
        foreach (var feature in _availableFeatures)
        {
            feature.OnFinish();
        }

        if (!_configurationWizardService.ConfiguredOnce)
        {
            MediatorService.Publish(new OpenGenericWindowMessage(typeof(FiltersWindow)));
        }
        _configurationWizardService.MarkFeaturesSeen();

    }

    public override void Invalidate()
    {
    }

    public override FilterConfiguration? SelectedConfiguration => null;
}