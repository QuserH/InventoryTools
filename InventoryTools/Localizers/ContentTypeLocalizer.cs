using AllaganLib.Shared.Extensions;
using Lumina.Excel.Sheets;
using InventoryTools.Localization;

namespace InventoryTools.Localizers;

public class ContentTypeLocalizer : ILocalizer<ContentType>
{
    public string Format(ContentType instance)
    {
        return instance.RowId switch
        {
            20 => "冒险者行会",
            22 => "Seasonal",
            23 => "白云崖",
            39 => "最终乐章",
            _ => instance.Name.ToImGuiString()
        };
    }
}
