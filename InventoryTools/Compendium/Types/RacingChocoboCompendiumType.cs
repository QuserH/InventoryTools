using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.Shared.Extensions;
using DalaMock.Host.Mediator;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Models;
using InventoryTools.Compendium.Services;
using InventoryTools.Ui;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using InventoryTools.Localization;

namespace InventoryTools.Compendium.Types;

public class RacingChocoboItemCompendiumType : CompendiumType<RacingChocoboItem>
{
    private readonly ExcelSheet<RacingChocoboItem> _racingChocoboItemSheet;

    public RacingChocoboItemCompendiumType(
        CompendiumTable<RacingChocoboItem>.Factory tableFactory,
        CompendiumColumnBuilder<RacingChocoboItem>.Factory columnBuilder,
        CompendiumViewBuilder.Factory viewBuilderFactory,
        ExcelSheet<RacingChocoboItem> racingChocoboItemSheet
    ) : base(tableFactory, columnBuilder, viewBuilderFactory)
    {
        _racingChocoboItemSheet = racingChocoboItemSheet;
    }

    public override string Singular => LocalizationService.Ui("Racing Chocobo Item");
    public override string Plural => LocalizationService.Ui("Racing Chocobo Items");
    public override string Description => LocalizationService.Ui("Items used for Racing Chocobo training and breeding.");
    public override string Key => "racingChocoboItems";

    public override (string?, uint?) Icon => (null, 72);

    public override ICompendiumTable<WindowState, MessageBase> BuildTable()
    {
        return Factory.Invoke(new()
        {
            Key = Key,
            Name = Plural,
            Columns = BuiltColumns,
            CompendiumType = this,
        });
    }

    public override string? GetName(RacingChocoboItem row)
    {
        return row.Item.ValueNullable?.Name.ToImGuiString();
    }

    public override string? GetSubtitle(RacingChocoboItem row)
    {
        return GetCategoryName(row.Category);
    }

    public override (string?, uint?) GetIcon(RacingChocoboItem row)
    {
        return (null, row.Item.ValueNullable?.Icon ?? 0);
    }

    public override uint GetRowId(RacingChocoboItem row)
    {
        return row.RowId;
    }

    public override RacingChocoboItem GetRow(uint row)
    {
        return _racingChocoboItemSheet.GetRow(row);
    }

    public override bool HasRow(uint rowId)
    {
        if (rowId == 0)
        {
            return false;
        }
        return _racingChocoboItemSheet.HasRow(rowId);
    }

    public override List<RacingChocoboItem> GetRows()
    {
        return _racingChocoboItemSheet.Where(c => c.RowId != 0 && c.Item.RowId != 0).ToList();
    }

    public override void BuildColumns(CompendiumColumnBuilder<RacingChocoboItem> builder)
    {
        builder.AddItemColumn(new()
        {
            Key = "icon",
            Name = LocalizationService.Ui("##Icon"),
            HelpText = LocalizationService.Ui("Item icon"),
            Version = "1.0",
            ValueSelector = row => row.Item.RowId,
        });

        builder.AddStringColumn(new()
        {
            Key = "name",
            Name = LocalizationService.Ui("Name"),
            Version = "1.0",
            HelpText = LocalizationService.Ui("The name of the item"),
            ValueSelector = r => r.Item.ValueNullable?.Name.ToImGuiString()
        });

        builder.AddStringColumn(new()
        {
            Key = "category",
            Name = LocalizationService.Ui("Category"),
            Version = "1.0",
            HelpText = LocalizationService.Ui("The category of the item"),
            ValueSelector = r => GetCategoryName(r.Category)
        });

        builder.AddIntegerColumn(new()
        {
            Key = "rank",
            Name = LocalizationService.Ui("Rank"),
            Version = "1.0",
            HelpText = LocalizationService.Ui("The rank of the item"),
            ValueSelector = r => r.Unknown1.ToString()
        });
    }

    public override void BuildViewFields(
        CompendiumViewBuilder viewBuilder,
        RacingChocoboItem row
    )
    {
        var item = row.Item.ValueNullable;

        viewBuilder.Title = item?.Name.ToImGuiString() ?? LocalizationService.Ui("Unknown");
        viewBuilder.Subtitle = GetCategoryName(row.Category);
        viewBuilder.Icon = item?.Icon ?? 0;

        viewBuilder.AddInfoTableSection(new()
        {
            SectionKey = "info",
            SectionName = LocalizationService.Ui("Info"),
            Items =
            [
                (LocalizationService.Ui("Category"), GetCategoryName(row.Category), true),
                (LocalizationService.Ui("Rank"), row.Unknown1.ToString(), true),
                (LocalizationService.Ui("Item ID"), row.Item.RowId.ToString(), true),
            ]
        });
    }

    public override string GetDefaultGrouping()
    {
        return "category";
    }

    public override List<ICompendiumGrouping>? GetGroupings()
    {
        return new()
        {
            new CompendiumGrouping<RacingChocoboItem>()
            {
                Key = "category",
                Name = LocalizationService.Ui("Category"),
                GroupFunc = r => r.Category,
                GroupMapping = r => GetCategoryName((byte)r)
            }
        };
    }

    public override Type ViewRedirection => typeof(ItemWindow);

    private static string GetCategoryName(byte category)
    {
        return category switch
        {
            1 => LocalizationService.Ui("Proof of Coverings"),
            2 => LocalizationService.Ui("Registration Forms"),
            3 => LocalizationService.Ui("Retired Registration Forms"),
            4 => LocalizationService.Ui("Covering Permission Forms"),
            5 => LocalizationService.Ui("Sack of Feeds"),
            6 => LocalizationService.Ui("Training Manuals"),
            7 => LocalizationService.Ui("Ability Reset Tonics"),
            _ => LocalizationService.Ui("Unknown")
        };
    }
}