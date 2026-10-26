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

public class AirshipRoutesCompendiumType : CompendiumType<AirshipExplorationPointRow>
{
    private readonly AirshipExplorationPointSheet _airshipExplorationPointSheet;

    public AirshipRoutesCompendiumType(AirshipExplorationPointSheet airshipExplorationPointSheet, CompendiumTable<AirshipExplorationPointRow>.Factory tableFactory, CompendiumColumnBuilder<AirshipExplorationPointRow>.Factory columnBuilder, CompendiumViewBuilder.Factory viewBuilderFactory) : base(tableFactory, columnBuilder, viewBuilderFactory)
    {
        _airshipExplorationPointSheet = airshipExplorationPointSheet;
    }

    public override string? GetName(AirshipExplorationPointRow row)
    {
        return row.Base.Name.ToImGuiString();
    }

    public override string? GetSubtitle(AirshipExplorationPointRow row)
    {
        return null;
    }

    public override (string?, uint?) GetIcon(AirshipExplorationPointRow row)
    {
        return (null, Icons.AirshipIcon);
    }

    public override uint GetRowId(AirshipExplorationPointRow row)
    {
        return row.RowId;
    }

    public override AirshipExplorationPointRow? GetRow(uint row)
    {
        return _airshipExplorationPointSheet.GetRowOrDefault(row);
    }

    public override bool HasRow(uint rowId)
    {
        return _airshipExplorationPointSheet.GetRowOrDefault(rowId) != null;
    }

    public override List<AirshipExplorationPointRow> GetRows()
    {
        return _airshipExplorationPointSheet.Where(c => c.Base.RowId != 0 && c.Base.Name.ToImGuiString() != "").ToList();
    }

    public override void BuildColumns(CompendiumColumnBuilder<AirshipExplorationPointRow> builder)
    {
        builder.AddCompendiumOpenViewColumn(new(){Key = "icon", Name = "图标", HelpText = "该航线的图标", Version = "14.0.3", CompendiumType = this, RowIdSelector = row => row.RowId, ValueSelector = this.GetIcon});
        builder.AddStringColumn(new (){Key = "name", Name = "名称", HelpText = "该航线的名称", Version = "14.0.3", ValueSelector = row => row.Base.Name.ToImGuiString()});
        builder.AddStringColumn(new (){Key = "unlock", Name = "解锁航线", HelpText = "解锁此航线的航线名称", Version = "14.0.3", ValueSelector = row => row.Unlock?.Base.Name.ToImGuiString() ?? ""});
        builder.AddIntegerColumn(new(){Key = "rankrequired", Name = "所需等级", HelpText = "该航线所需等级", Version = "14.0.3", ValueSelector =row => row.Base.RankReq.ToString()});
        builder.AddIntegerColumn(new(){Key = "cerelumrequired", Name = "所需青磷水", HelpText = "该航线所需青磷水", Version = "14.0.3", ValueSelector =row => row.Base.CeruleumTankReq.ToString()});
        builder.AddIntegerColumn(new(){Key = "surveillancerequired", Name = "所需监视性能", HelpText = "该航线所需监视性能", Version = "14.0.3", ValueSelector =row => row.Base.SurveillanceReq.ToString()});
        builder.AddItemsColumn(new(){Key = "drops", Name = "掉落物品", HelpText = "该飞空艇航线的掉落物", Version = "14.0.3", ValueSelector = row => row.DropItems, ColumnFlags = ImGuiTableColumnFlags.WidthFixed});
    }

    public override void BuildViewFields(CompendiumViewBuilder viewBuilder, AirshipExplorationPointRow row)
    {
        viewBuilder.SetupDefaults(this, row);
        var information = new List<(string Header, string Value, bool IsVisible)>
        {
            ("所需等级", row.Base.RankReq.ToString(), true),
            ("所需青磷水", row.Base.CeruleumTankReq.ToString(), true),
            ("所需监视性能", row.Base.SurveillanceReq.ToString(), true)
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
            RelatedRefs = _airshipExplorationPointSheet.Where(c => c.UnlockId != null && c.UnlockId == row.RowId).Select(c => c.Base.AsUntypedRowRef()).ToList(),
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
            Key = "airships",
            Name = Plural,
            Columns = BuiltColumns,
            CompendiumType = this,
        });
    }

    public override List<Type>? RelatedTypes => [typeof(AirshipExplorationPoint)];

    public override string Singular => "飞空艇航线";
    public override string Plural => "飞空艇航线";
    public override string Description => "部队飞空艇飞行过的航线";
    public override string Key => "airships";
    public override (string?, uint?) Icon => (null, Icons.AirshipIcon);
}