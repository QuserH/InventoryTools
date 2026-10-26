using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.Shared.Extensions;
using DalaMock.Host.Mediator;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Models;
using InventoryTools.Compendium.Sections.Options;
using InventoryTools.Compendium.Services;
using InventoryTools.Compendium.Types.Extra;
using InventoryTools.Localizers;
using InventoryTools.Localization;

public class ChocoboItemCompendiumType : CompendiumType<ChocoboItem>
{
    private readonly ChocoboItemIterator _iterator;
    private readonly ILocalizer<ChocoboItemSourceType> _itemSourceTypeLocalizer;

    private List<ChocoboItem>? _rows;

    public ChocoboItemCompendiumType(
        CompendiumTable<ChocoboItem>.Factory tableFactory,
        CompendiumColumnBuilder<ChocoboItem>.Factory columnBuilder,
        CompendiumViewBuilder.Factory viewBuilderFactory,
        ChocoboItemIterator iterator,
        ILocalizer<ChocoboItemSourceType> itemSourceTypeLocalizer
    ) : base(tableFactory, columnBuilder, viewBuilderFactory)
    {
        _iterator = iterator;
        _itemSourceTypeLocalizer = itemSourceTypeLocalizer;
    }

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

    public override string Singular => "陆行鸟物品";
    public override string Plural => "陆行鸟物品";
    public override string Description => "陆行鸟伙伴消耗或装备的物品。";
    public override string Key => "chocoboItems";

    public override (string?, uint?) Icon => (null, 74);

    public override List<ChocoboItem> GetRows()
    {
        return _iterator.ToList();
    }

    public override ChocoboItem GetRow(uint row)
    {
        return _iterator.FirstOrDefault(r => r.RowId == row);
    }

    public override bool HasRow(uint rowId)
    {
        return _iterator.Any(r => r.RowId == rowId);
    }

    public override string? GetName(ChocoboItem row)
    {
        return row.Item.NameString;
    }

    public override string? GetSubtitle(ChocoboItem row)
    {
        return _itemSourceTypeLocalizer.Format(row.SourceType);
    }

    public override (string?, uint?) GetIcon(ChocoboItem row)
    {
        return (null, row.Item.Icon);
    }

    public override uint GetRowId(ChocoboItem row)
    {
        return row.RowId;
    }

    public override void BuildColumns(
        CompendiumColumnBuilder<ChocoboItem> builder)
    {
        builder.AddItemColumn(new()
        {
            Key = "icon",
            Name = "##Icon",
            HelpText = "该陆行鸟物品的图标。",
            Version = "1",
            ValueSelector = row => row.Item.RowId,
        });

        builder.AddStringColumn(new()
        {
            Key = "name",
            Name = "名称",
            HelpText = "该陆行鸟物品的名称。",
            Version = "1",
            ValueSelector = r => r.Item.NameString
        });

        builder.AddStringColumn(new()
        {
            Key = "source",
            Name = "来源",
            HelpText = "该陆行鸟物品的来源。",
            Version = "1",
            ValueSelector = r => _itemSourceTypeLocalizer.Format(r.SourceType)
        });

        builder.AddBooleanColumn(new()
        {
            Key = "training_item",
            Name = "训练物品？",
            HelpText = "该物品是否用于训练？",
            Version = "1",
            ValueSelector = r => r.BuddyItem?.Value.UseTraining
        });

        builder.AddBooleanColumn(new()
        {
            Key = "field_item",
            Name = "野外物品？",
            HelpText = "该物品是否用于野外？",
            Version = "1",
            ValueSelector = r => r.BuddyItem?.Value.UseField
        });

        builder.AddBooleanColumn(new()
        {
            Key = "recolor_item",
            Name = "染色物品？",
            HelpText = "该物品是否用于改变陆行鸟颜色？",
            Version = "1",
            ValueSelector = r => r.BuddyItem?.Value.Unknown0
        });
    }

    public override void BuildViewFields(
        CompendiumViewBuilder viewBuilder,
        ChocoboItem row)
    {
        viewBuilder.Title = row.Item.NameString;
        viewBuilder.Icon = row.Item.Icon;
        viewBuilder.Subtitle = _itemSourceTypeLocalizer.Format(row.SourceType);

        var useTraining = row.BuddyItem?.Value.UseTraining;
        var useField = row.BuddyItem?.Value.UseField;
        var dyeField = row.BuddyItem?.Value.Unknown0;
        viewBuilder.AddInfoTableSection(new()
        {
            SectionKey = "info",
            SectionName = "信息",
            Items =
            [
                ("来源", _itemSourceTypeLocalizer.Format(row.SourceType), true),
                ("训练物品？", TriStateFormatted(useTraining), row.BuddyItem != null),
                ("野外物品？", TriStateFormatted(useField), row.BuddyItem != null),
                ("染色物品？", TriStateFormatted(dyeField), row.BuddyItem != null),
            ]
        });
    }

    public override string GetDefaultGrouping()
    {
        return "source";
    }

    public override List<ICompendiumGrouping>? GetGroupings()
    {
        return new()
        {
            new CompendiumGrouping<ChocoboItem>()
            {
                Key = "source",
                Name = "来源",
                GroupFunc = r => r.SourceType,
                GroupMapping = o =>
                {
                    var itemSourceType = (ChocoboItemSourceType)o;
                    return _itemSourceTypeLocalizer.Format(itemSourceType);
                }
            }
        };
    }
}
