using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings.Abstract
{
    public abstract class StringSetting : Setting<string>
    {
        public StringSetting(ILogger logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override bool HasValueSet(InventoryToolsConfiguration configuration)
        {
            return CurrentValue(configuration) != "";
        }

        public override void Draw(InventoryToolsConfiguration configuration, string? customName, bool? disableReset,
            bool? disableColouring)
        {
            var value = CurrentValue(configuration) ?? "";
            if (disableColouring != true && HasValueSet(configuration))
            {
                ImGui.PushStyleColor(ImGuiCol.Text,ImGuiColors.HealerGreen);
                ImGui.LabelText(LocalizationService.Ui("##") + Key + "Label", customName ?? Name);
                ImGui.PopStyleColor();
            }
            else
            {
                ImGui.LabelText(LocalizationService.Ui("##") + Key + "Label", customName ?? Name);
            }
            ImGui.SameLine();
            if (ImGui.InputText(LocalizationService.Ui("##")+Key+"Input", ref value, 500))
            {
                UpdateFilterConfiguration(configuration, value);
            }
            ImGui.SameLine();
            ImGuiService.HelpMarker(HelpText, Image, ImageSize);
            if (disableReset != true && HasValueSet(configuration))
            {
                ImGui.SameLine();
                if (ImGui.Button(LocalizationService.Ui(LocalizationService.Ui("Reset##")) + Key + "Reset"))
                {
                    Reset(configuration);
                }
            }
        }
    }
}