using System.Numerics;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Settings.Abstract
{
    public abstract class ColorSetting : Setting<Vector4>
    {
        public ColorSetting(ILogger logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override void Draw(InventoryToolsConfiguration configuration, string? customName, bool? disableReset,
            bool? disableColouring)
        {
            var value = CurrentValue(configuration);

            if (ImGui.ColorEdit4("##" + Key + "Color", ref value, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel))
            {
                UpdateFilterConfiguration(configuration, value);
            }
            ImGui.SameLine();
            if (HasValueSet(configuration) && value.W == 0)
            {
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.DalamudRed, "透明度当前设置为 0，将不可见。");
            }
            ImGui.SameLine();
            if (disableColouring != true && HasValueSet(configuration))
            {
                ImGui.PushStyleColor(ImGuiCol.Text,ImGuiColors.HealerGreen);
                ImGui.LabelText("##" + Key + "标签", customName ?? Name);
                ImGui.PopStyleColor();
            }
            else
            {
                ImGui.LabelText("##" + Key + "标签", customName ?? Name);
            }
            ImGui.SameLine();
            ImGuiService.HelpMarker(HelpText, Image, ImageSize);
            if (disableReset != true && HasValueSet(configuration))
            {
                ImGui.SameLine();
                if (ImGui.Button("重置##" + Key + "重置"))
                {
                    Reset(configuration);
                }
            }
        }
    }
}