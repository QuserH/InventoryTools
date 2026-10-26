using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using AllaganLib.Shared.Misc;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Models;
using InventoryTools.Compendium.Sections;
using InventoryTools.Compendium.Sections.Options;
using InventoryTools.Compendium.Services;
using Lumina.Excel.Sheets;
using InventoryTools.Localization;

namespace InventoryTools.Compendium.Types;

public class SubmarineRoutesCompendiumType : CompendiumType<SubmarineExplorationRow>
{
    private readonly SubmarineExplorationSheet _submarineExplorationSheet;

    public SubmarineRoutesCompendiumType(SubmarineExplorationSheet submarineExplorationSheet, CompendiumTable<SubmarineExplorationRow>.Factory tableFactory, CompendiumColumnBuilder<SubmarineExplorationRow>.Factory columnBuilder, CompendiumViewBuilder.Factory viewBuilderFactory) : base(tableFactory, columnBuilder, viewBuilderFactory)
    {
        _submarineExplorationSheet = submarineExplorationSheet;
    }

    public override string? GetName(SubmarineExplorationRow row)
    {
        return row.Base.Destination.ToImGuiString();
    }

    public override string? GetSubtitle(SubmarineExplorationRow row)
    {
        return null;
    }

    public override (string?, uint?) GetIcon(SubmarineExplorationRow row)
    {
        return (null, Icons.SubmarineIcon);
    }

    public override uint GetRowId(SubmarineExplorationRow row)
    {
        return row.RowId;
    }

    public override SubmarineExplorationRow? GetRow(uint row)
    {
        return _submarineExplorationSheet.GetRowOrDefault(row);
    }

    public override bool HasRow(uint rowId)
    {
        return _submarineExplorationSheet.GetRowOrDefault(rowId) != null;
    }

    public override List<SubmarineExplorationRow> GetRows()
    {
        return _submarineExplorationSheet.Where(c => c.Base.RowId != 0 && c.Base.Destination.ToImGuiString() != "").ToList();
    }

    public override void BuildColumns(CompendiumColumnBuilder<SubmarineExplorationRow> builder)
    {
        builder.AddCompendiumOpenViewColumn(new(){Key = "icon", Name = "图标", HelpText = "该航线的图标", Version = "14.0.3", CompendiumType = this, RowIdSelector = row => row.RowId, ValueSelector = this.GetIcon});
        builder.AddStringColumn(new (){Key = "name", Name = "名称", HelpText = "该航线的名称", Version = "14.0.3", ValueSelector = row => row.Base.Destination.ToImGuiString()});
        builder.AddStringColumn(new (){Key = "unlock", Name = "解锁航线", HelpText = "解锁此航线的航线名称", Version = "14.0.3", ValueSelector = row => row.Unlock?.Base.Destination.ToImGuiString() ?? ""});
        builder.AddIntegerColumn(new(){Key = "rankrequired", Name = "所需等级", HelpText = "该航线所需等级", Version = "14.0.3", ValueSelector =row => row.Base.RankReq.ToString()});
        builder.AddIntegerColumn(new(){Key = "cerelumrequired", Name = "所需青磷水", HelpText = "该航线所需青磷水", Version = "14.0.3", ValueSelector =row => row.Base.CeruleumTankReq.ToString()});
        builder.AddItemsColumn(new(){Key = "drops", Name = "掉落物品", HelpText = "该潜水艇航线的掉落物", Version = "14.0.3", ValueSelector = row => row.DropItems, ColumnFlags = ImGuiTableColumnFlags.WidthFixed});
    }

    public override void BuildViewFields(CompendiumViewBuilder viewBuilder, SubmarineExplorationRow row)
    {
        viewBuilder.SetupDefaults(this, row);
        var information = new List<(string Header, string Value, bool IsVisible)>
        {
            ("所需等级", row.Base.RankReq.ToString(), true),
            ("所需青磷水", row.Base.CeruleumTankReq.ToString(), true),
        };
        viewBuilder.AddInfoTableSection(new InfoTableSectionOptions()
        {
            SectionKey = "information",
            SectionName = "信息",
            Items = information.AsReadOnly()
        });
        if (row.Unlock != null)
        {
            viewBuilder.AddSingleRowRefSection(new SingleRowRefSectionOptions()
            {
                SectionKey = "unlocked_via",
                SectionName = "解锁方式",
                RelatedRef = row.Unlock.Base.AsUntypedRowRef()
            });
        }
        viewBuilder.AddCollectionRowRefSection(new CollectionRowRefSectionOptions()
        {
            RelatedRefs = _submarineExplorationSheet.Where(c => c.UnlockId != null && c.UnlockId == row.RowId).Select(c => c.Base.AsUntypedRowRef()).ToList(),
            SectionKey = "unlocks",
            SectionName = "解锁内容",
            HideWhenEmpty = true
        });

        viewBuilder.AddItemListSection(new ItemListSectionOptions()
        {
            Items = row.DropItems.Select(c => new ItemInfo(c)).ToList(),
            SectionKey = "potential_drops",
            SectionName = "可能掉落"
        });
    }

    public override ICompendiumTable<WindowState, MessageBase> BuildTable()
    {
        return Factory.Invoke(new()
        {
            Key = "submarines",
            Name = Plural,
            Columns = BuiltColumns,
            CompendiumType = this,
        });
    }

    public override List<Type>? RelatedTypes => [typeof(SubmarineExploration)];

    public override string Singular => "潜水艇航线";
    public override string Plural => "潜水艇航线";
    public override string Description => "部队潜水艇航行过的航线";
    public override string Key => "submarines";
    public override (string?, uint?) Icon => (null, Icons.SubmarineIcon);
}