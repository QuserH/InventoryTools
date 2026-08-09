using Dalamud.Interface.Colors;
using Dalamud.Bindings.ImGui;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using OtterGui.Raii;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Filters.Abstract
{
    using System.Collections.Generic;

    public abstract class StringFilter : Filter<string>
    {

        public StringFilter(ILogger logger, ImGuiService imGuiService) : base(logger, imGuiService)
        {
        }
        public override string DefaultValue { get; set; } = "";

        public override bool HasValueSet(FilterConfiguration configuration)
        {
            return CurrentValue(configuration) != "";
        }

        public override string CurrentValue(FilterConfiguration configuration)
        {
            return (configuration.GetStringFilter(Key) ?? "").Trim();
        }

        public override void Draw(FilterConfiguration configuration)
        {
            var value = CurrentValue(configuration) ?? "";
            if (HasValueSet(configuration))
            {
                ImGui.PushStyleColor(ImGuiCol.Text,ImGuiColors.HealerGreen);
                ImGui.LabelText(LocalizationService.Ui("##") + Key + "Label", GetName(configuration) + ":");
                ImGui.PopStyleColor();
            }
            else
            {
                ImGui.LabelText(LocalizationService.Ui("##") + Key + "Label", GetName(configuration) + ":");
            }

            ImGui.Indent();
            using (ImRaii.PushColor(ImGuiCol.Text, ImGuiColors.DalamudGrey))
            {
                ImGui.PushTextWrapPos();
                ImGui.TextUnformatted(GetHelpText(configuration));
                ImGui.PopTextWrapPos();
            }

            ImGui.SetNextItemWidth(InputSize);
            if (ImGui.InputText(LocalizationService.Ui("##")+Key+"Input", ref value, 500))
            {
                UpdateFilterConfiguration(configuration, value);
            }
            if (this.ShowOperatorTooltip)
            {
                ImGui.SameLine();
                ImGuiService.HelpMarker(new List<string>()
                {
                    LocalizationService.Ui("When searching the following operators can be used to compare: "),
                    "",
                    LocalizationService.Ui(">, >=, <, <=, =, for numerical comparisons") ,
                    LocalizationService.Ui("=, for exact comparisons"),
                    LocalizationService.Ui("!, for inequality comparisons"),
                    LocalizationService.Ui("||, search multiple expressions using OR"),
                    LocalizationService.Ui("&&, search multiple expressions using AND")
                });
            }

            if (HasValueSet(configuration) && ShowReset)
            {
                ImGui.SameLine();
                if (ImGui.Button(LocalizationService.Ui(LocalizationService.Ui("Reset##")) + Key + "Reset"))
                {
                    ResetFilter(configuration);
                }
            }
            ImGui.Unindent();
        }

        public override void UpdateFilterConfiguration(FilterConfiguration configuration, string newValue)
        {
            configuration.UpdateStringFilter(Key, newValue);
        }

        public override void ResetFilter(FilterConfiguration configuration)
        {
            UpdateFilterConfiguration(configuration, DefaultValue);
        }
    }
}