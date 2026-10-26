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

    public override string Singular => "竞速陆行鸟物品";
    public override string Plural => "竞速陆行鸟物品";
    public override string Description => "用于竞速陆行鸟训练和培育的物品。";
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
            Name = "##Icon",
            HelpText = "物品图标",
            Version = "1.0",
            ValueSelector = row => row.Item.RowId,
        });

        builder.AddStringColumn(new()
        {
            Key = "name",
            Name = "名称",
            Version = "1.0",
            HelpText = "物品的名称",
            ValueSelector = r => r.Item.ValueNullable?.Name.ToImGuiString()
        });

        builder.AddStringColumn(new()
        {
            Key = "category",
            Name = "分类",
            Version = "1.0",
            HelpText = "物品的分类",
            ValueSelector = r => GetCategoryName(r.Category)
        });

        builder.AddIntegerColumn(new()
        {
            Key = "rank",
            Name = "等级",
            Version = "1.0",
            HelpText = "物品的等级",
            ValueSelector = r => r.Unknown1.ToString()
        });
    }

    public override void BuildViewFields(
        CompendiumViewBuilder viewBuilder,
        RacingChocoboItem row
    )
    {
        var item = row.Item.ValueNullable;

        viewBuilder.Title = item?.Name.ToImGuiString() ?? "未知";
        viewBuilder.Subtitle = GetCategoryName(row.Category);
        viewBuilder.Icon = item?.Icon ?? 0;

        viewBuilder.AddInfoTableSection(new()
        {
            SectionKey = "info",
            SectionName = "信息",
            Items =
            [
                ("分类", GetCategoryName(row.Category), true),
                ("等级", row.Unknown1.ToString(), true),
                ("物品 ID", row.Item.RowId.ToString(), true),
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
                Name = "分类",
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
            1 => "骑乘许可证明",
            2 => "注册表",
            3 => "退役注册表",
            4 => "骑乘许可表",
            5 => "饲料袋",
            6 => "训练手册",
            7 => "能力重置药",
            _ => "未知"
        };
    }
}
