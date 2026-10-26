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
            if (ImGui.InputText("##"+Key+"Input", ref value, 500))
            {
                UpdateFilterConfiguration(configuration, value);
            }
            if (this.ShowOperatorTooltip)
            {
                ImGui.SameLine();
                ImGuiService.HelpMarker(new List<string>()
                {
                    "搜索时可用以下运算符比较：",
                    "",
                    ">, >=, <, <=, =，用于数值比较" ,
                    "=，用于精确比较",
                    "!，用于不等比较",
                    "||，使用或连接多个表达式",
                    "&&，使用AND连接多个表达式"
                });
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