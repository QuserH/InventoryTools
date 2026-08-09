using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings.Abstract
{
    public abstract class IntegerSetting : Setting<int>
    {
        public IntegerSetting(ILogger logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override void Draw(InventoryToolsConfiguration configuration, string? customName, bool? disableReset,
            bool? disableColouring)
        {
            var value = CurrentValue(configuration).ToString();
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
            ImGui.SetNextItemWidth(InputSize);
            if (ImGui.InputText(LocalizationService.Ui("##")+Key+"Input", ref value, 100, ImGuiInputTextFlags.CharsDecimal))
            {
                int parsedNumber;
                if(int.TryParse(value, out parsedNumber))
                {
                    UpdateFilterConfiguration(configuration, parsedNumber);
                }
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