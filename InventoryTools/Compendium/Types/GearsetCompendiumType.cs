using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets;
using CriticalCommonLib.Models;
using DalaMock.Host.Mediator;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Models;
using InventoryTools.Compendium.Sections;
using InventoryTools.Compendium.Services;
using LuminaSupplemental.Excel.Model;
using InventoryTools.Localization;

namespace InventoryTools.Compendium.Types;

public class GearsetCompendiumType : CompendiumType<Gearset>
{
    private readonly ItemListSection.Factory _itemListSectionFactory;
    private readonly List<Gearset> _gearsets;
    private readonly ItemSheet _itemSheet;
    private readonly ItemInfoCache _itemInfoCache;
    private readonly CompendiumMenuBuilder _menuBuilder;

    public GearsetCompendiumType(CompendiumTable<Gearset>.Factory tableFactory,
        CompendiumColumnBuilder<Gearset>.Factory columnBuilder,
        CompendiumViewBuilder.Factory viewBuilderFactory,
        ItemListSection.Factory  itemListSectionFactory,
        List<Gearset> gearsets,
        ItemSheet itemSheet,
        ItemInfoCache itemInfoCache,
        CompendiumMenuBuilder menuBuilder) : base(tableFactory,
        columnBuilder,
        viewBuilderFactory)
    {
        _itemListSectionFactory = itemListSectionFactory;
        _gearsets = gearsets;
        _itemSheet = itemSheet;
        _itemInfoCache = itemInfoCache;
        _menuBuilder = menuBuilder;
    }

    public override ICompendiumTable<WindowState, MessageBase> BuildTable()
    {
        return Factory.Invoke(new()
        {
            Key = "gearsets",
            Name = Plural,
            Columns = BuiltColumns,
            CompendiumType = this,
            BuildContextMenu = BuildContextMenu
        });
    }

    public override string? GetName(Gearset row)
    {
        return row.Name;
    }

    public override string? GetSubtitle(Gearset row)
    {
        return row.Items.Count + LocalizationService.Ui(" items");
    }

    public override (string?, uint?) GetIcon(Gearset row)
    {
        return (null, Icons.ArmorIcon);
    }

    public override uint GetRowId(Gearset row)
    {
        return (uint)GetRows().IndexOf(row);
    }

    private List<MessageBase> BuildContextMenu(Gearset arg)
    {
        _menuBuilder.Header(arg.Name);
        _menuBuilder.TryOn(arg.Items, LocalizationService.Ui("Try on Gearset"));
        _menuBuilder.NewLine();
        _menuBuilder.Header(LocalizationService.Ui("Gear Pieces"));
        _menuBuilder.Items(arg.Items);
        _menuBuilder.GroupedItems(arg.Items, LocalizationService.Ui("All Gear Pieces"));
        return [];
    }

    public override string Singular => LocalizationService.Ui("Gearset");
    public override string Plural => LocalizationService.Ui("Gearsets");
    public override string Description => LocalizationService.Ui("Gearsets based on Eorzea Collection's organizing.");
    public override string Key => "gearsets";
    public override (string?, uint?) Icon => (null, Icons.ArmorIcon);


    public override Gearset GetRow(uint row)
    {
        return _gearsets[(int)row];
    }

    public override bool HasRow(uint rowId)
    {
        return (int)rowId >= 0 && (int)rowId < _gearsets.Count;
    }

    public override List<Gearset> GetRows()
    {
        return _gearsets;
    }

    public override void BuildColumns(CompendiumColumnBuilder<Gearset> builder)
    {
        builder.AddCompendiumOpenViewColumn(new(){Key = "icon", Name = LocalizationService.Ui("##Icon"), HelpText = LocalizationService.Ui("The icon of the gearset"), Version = "14.0.3", ValueSelector = row => (null, Icons.ArmorIcon), CompendiumType = this, RowIdSelector = row => (uint)_gearsets.IndexOf(row)});
        builder.AddStringColumn(new (){Key = "name", Name = LocalizationService.Ui("Name"), HelpText = LocalizationService.Ui("The name of the gearset"), Version = "14.0.3", ValueSelector = row => row.Name});
        builder.AddItemSourcesColumn(new() { Key = "sources", Name = LocalizationService.Ui("Sources"), HelpText = LocalizationService.Ui("The combined sources for the gearset."), Version = "14.0.3", ValueSelector = gearset => gearset.Items.Where(c => c.RowId != 0).SelectMany(c => _itemInfoCache.GetItemSources(c.RowId) ?? []).ToList()});
        builder.AddStringColumn(new (){Key = "patch", Name = LocalizationService.Ui("Patch"), HelpText = LocalizationService.Ui("The patch the gearset was added."), Version = "14.0.3", ValueSelector = gearset => string.Join(", ", gearset.Items.Where(c => c.RowId != 0).Select(c => _itemSheet.GetRow(c.RowId).Patch.ToString(CultureInfo.InvariantCulture)).Distinct())});
        builder.AddIntegerColumn(new (){Key = "ilvl", Name = LocalizationService.Ui("Item Level"), HelpText = LocalizationService.Ui("The highest item level (iLvl) across all pieces in the gearset."), Version = "15.0.6", Width = 60, ValueSelector = gearset => { var max = gearset.Items.Where(c => c.RowId != 0).Select(c => (int)_itemSheet.GetRow(c.RowId).Base.LevelItem.RowId).DefaultIfEmpty(0).Max(); return max == 0 ? null : max.ToString(); }});
        builder.AddIntegerColumn(new (){Key = "equip_level", Name = LocalizationService.Ui("Equip Level"), HelpText = LocalizationService.Ui("The highest required equip level across all pieces in the gearset."), Version = "15.0.6", Width = 60, ValueSelector = gearset => { var max = gearset.Items.Where(c => c.RowId != 0).Select(c => (int)_itemSheet.GetRow(c.RowId).Base.LevelEquip).DefaultIfEmpty(0).Max(); return max == 0 ? null : max.ToString(); }});
        for (int i = 0; i < 12; i++)
        {
            var index = i;
            builder.AddItemColumn(new(){Key = "item" + i, Name = LocalizationService.Ui("Item ") + (i + 1), HelpText = LocalizationService.Ui("The item"), Version = "14.0.3", ValueSelector = row => row.Items[index].RowId});
        }
    }

    public override void BuildViewFields(CompendiumViewBuilder viewBuilder, Gearset row)
    {
        var itemCount = row.Items.Count(c => c.RowId != 0);
        viewBuilder.Title = row.Name;
        viewBuilder.Icon = Icons.ArmorIcon;
        viewBuilder.Subtitle = itemCount + " " + LocalizationService.Ui(row.Items.Count == 1 ? " item" : " items");
        viewBuilder.AddLink("https://ffxiv.eorzeacollection.com/gearset/" + row.Key, LocalizationService.Ui("Open in Eorzea Collection"), "ec");
        viewBuilder.AddSection(_itemListSectionFactory.Invoke(new(){SectionKey = "set_items", SectionName = LocalizationService.Ui("Set Items"), Items = row.Items.Where(c => c.RowId != 0).Select(c => ItemInfo.Create(_itemSheet.GetRow(c.RowId)))}));
    }
}
