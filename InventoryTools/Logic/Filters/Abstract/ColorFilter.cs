using System.Numerics;
using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using OtterGui.Raii;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters.Abstract
{
    public abstract class ColorFilter : Filter<Vector4?>
    {
        public override Vector4? DefaultValue { get; set; } = null;

        public override bool HasValueSet(FilterConfiguration configuration)
        {
            return CurrentValue(configuration) != DefaultValue;
        }

        public override Vector4? CurrentValue(FilterConfiguration configuration)
        {
            return configuration.GetColorFilter(Key);
        }

        public override void Draw(FilterConfiguration configuration)
        {
            var value = CurrentValue(configuration);
            if (HasValueSet(configuration))
            {
                ImGui.PushStyleColor(ImGuiCol.Text,ImGuiColors.HealerGreen);
                ImGui.LabelText("##" + Key + "标签", GetName(configuration) + ":");
                ImGui.PopStyleColor();
            }
            else
            {
                ImGui.LabelText("##" + Key + "标签", GetName(configuration) + ":");
            }
            ImGui.Indent();
            using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey))
            {
                ImGui.PushTextWrapPos();
                ImGui.TextUnformatted(GetHelpText(configuration));
                ImGui.PopTextWrapPos();
            }
            ImGui.SetNextItemWidth(InputSize);
            if (value == null)
            {
                var isChecked = false;
                if (ImGui.Checkbox("覆盖颜色##" + Key + "ColorEnable", ref isChecked))
                {
                    if (isChecked)
                    {
                        UpdateFilterConfiguration(configuration, Vector4.Zero);
                    }
                }
            }
            else
            {
                var editValue = value.Value;
                if (ImGui.ColorEdit4("##" + Key + "Color", ref editValue,
                        ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.NoLabel))
                {
                    UpdateFilterConfiguration(configuration, editValue);
                }
            }

            if (HasValueSet(configuration))
            {
                ImGui.SameLine();
                if (ImGui.Button("清除颜色"))
                {
                    UpdateFilterConfiguration(configuration, null);
                }
            }
            if (HasValueSet(configuration) && value?.W == 0)
            {
                ImGui.SameLine();
                ImGui.TextColored(ImGuiColors.DalamudRed, "透明度当前设置为 0，将不可见。");
            }
            if (HasValueSet(configuration) && ShowReset)
            {
                ImGui.SameLine();
                if (ImGui.Button("重置##" + Key + "重置"))
                {
                    ResetFilter(configuration);
                }
            }
            ImGui.Unindent();
        }

        public override void UpdateFilterConfiguration(FilterConfiguration configuration, Vector4? newValue)
        {
            if (newValue.HasValue)
            {
                configuration.UpdateColorFilter(Key, newValue.Value);
            }
            else
            {
                configuration.RemoveColorFilter(Key);
            }
        }

        public override void ResetFilter(FilterConfiguration configuration)
        {
            UpdateFilterConfiguration(configuration, DefaultValue);
        }

        protected ColorFilter(ILogger logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
    }
}