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
            20 => LocalizationService.Ui("Hall of Novice"),
            22 => "Seasonal",
            23 => LocalizationService.Ui("The Diadem"),
            39 => LocalizationService.Ui("The Final Verse"),
            _ => instance.Name.ToImGuiString()
        };
    }
}
