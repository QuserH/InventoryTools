using System;
using LuminaSupplemental.Excel.Model;
using InventoryTools.Localization;

namespace InventoryTools.Localizers;

public class RelicToolCategoryLocalizer : ILocalizer<RelicToolCategory>
{
    public string Format(RelicToolCategory instance)
    {
        switch (instance)
        {
            case RelicToolCategory.Mastercraft:
                return "成就工具";
            case RelicToolCategory.Skysteel:
                return "天钢工具";
            case RelicToolCategory.Resplendent:
                return "天穹工具";
            case RelicToolCategory.Splendorous:
                return "莫雯卓越工具";
            case RelicToolCategory.Cosmic:
                return "宇宙工具";
        }
        return instance.ToString();
    }
}