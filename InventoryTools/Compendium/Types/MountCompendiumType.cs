using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Extensions;
using AllaganLib.Shared.Extensions;
using CriticalCommonLib.Models;
using DalaMock.Host.Mediator;
using Dalamud.Interface.Colors;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Models;
using InventoryTools.Compendium.Sections.Options;
using InventoryTools.Compendium.Services;
using InventoryTools.Ui;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using InventoryTools.Localization;

namespace InventoryTools.Compendium.Types;

public class MountCompendiumType : CompendiumType<Mount>
{
    private readonly IUnlockState _unlockState;
    private readonly ItemInfoCache _itemInfoCache;
    private readonly ExcelSheet<Item> _itemSheet;
    private readonly ExcelSheet<Mount> _mountSheet;
    private readonly ExcelSheet<MountTransient> _mountTransientSheet;

    private Dictionary<uint, uint>? _mountToItem;
    private Dictionary<uint, uint>? _itemToMount;

    public MountCompendiumType(
        IUnlockState unlockState,
        ItemInfoCache itemInfoCache,
        ExcelSheet<ItemAction> itemActionSheet,
        ExcelSheet<Item> itemSheet,
        ExcelSheet<Mount> mountSheet,
        ExcelSheet<MountTransient> mountTransientSheet,
        CompendiumTable<Mount>.Factory tableFactory,
        CompendiumColumnBuilder<Mount>.Factory columnBuilder,
        CompendiumViewBuilder.Factory viewBuilderFactory)
        : base(tableFactory, columnBuilder, viewBuilderFactory)
    {
        _unlockState = unlockState;
        _itemInfoCache = itemInfoCache;
        _itemSheet = itemSheet;
        _mountSheet = mountSheet;
        _mountTransientSheet = mountTransientSheet;
    }

    public Item? GetRelatedItem(uint mountId)
    {
        if (_mountToItem == null)
            CalculateMapping();

        return _mountToItem!.TryGetValue(mountId, out var value)
            ? _itemSheet.GetRow(value)
            : null;
    }

    private void CalculateMapping()
    {
        _mountToItem = new Dictionary<uint, uint>();
        _itemToMount = new Dictionary<uint, uint>();

        foreach (var item in _itemSheet)
        {
            if (item.ItemAction.RowId == 0)
                continue;

            var itemAction = item.ItemAction.Value;

            if (itemAction.Action.RowId == 1322)
            {
                var mountId = itemAction.Data[0];

                if (mountId != 0)
                {
                    _mountToItem.TryAdd(mountId, item.RowId);
                    _itemToMount.TryAdd(item.RowId, mountId);
                }
            }
        }
    }

    public override ICompendiumTable<WindowState, MessageBase> BuildTable()
    {
        return Factory.Invoke(new CompendiumTableOptions<Mount>()
        {
            Name = "坐骑",
            Columns = BuiltColumns,
            CompendiumType = this,
            Key = "mounts",
        });
    }

    public override string? GetName(Mount row)
    {
        return row.Singular.ToImGuiString().FirstCharToUpper();
    }

    public override string? GetSubtitle(Mount row)
    {
        return row.ExtraSeats > 0
            ? $"Seats: {row.ExtraSeats + 1}"
            : null;
    }

    public override (string?, uint?) GetIcon(Mount row)
    {
        return (null, row.Icon);
    }

    public override uint GetRowId(Mount row)
    {
        return row.RowId;
    }

    public override Mount GetRow(uint row)
    {
        return _mountSheet.GetRow(row);
    }

    public override List<Mount> GetRows()
    {
        return _mountSheet
            .Where(m => !m.Singular.IsEmpty && m.Icon != 0)
            .ToList();
    }

    public override void BuildColumns(CompendiumColumnBuilder<Mount> builder)
    {
        builder.AddCompendiumOpenViewColumn(new()
        {
            Key = "icon",
            Name = "##Icon",
            HelpText = "坐骑图标",
            Version = "14.1.2",
            ValueSelector = GetIcon,
            CompendiumType = this,
            RowIdSelector = r => r.RowId
        });

        builder.AddStringColumn(new()
        {
            Key = "name",
            Name = "名称",
            HelpText = "坐骑名称",
            Version = "14.1.2",
            ValueSelector = GetName
        });

        builder.AddBooleanColumn(new()
        {
            Key = "unlocked",
            Name = "已解锁",
            HelpText = "已解锁",
            Version = "14.1.2",
            ValueSelector = r => _unlockState.IsMountUnlocked(r)
        });


        builder.AddIntegerColumn(new()
        {
            Key = "seats",
            Name = "座位",
            HelpText = "该坐骑可乘坐几人？",
            Version = "14.1.2",
            ValueSelector = r => (r.ExtraSeats + 1).ToString()
        });

        builder.AddItemSourcesColumn(new()
        {
            Key = "sources",
            Name = "来源",
            HelpText = "坐骑来源",
            Version = "14.1.2",
            ValueSelector = r =>
            {
                var item = GetRelatedItem(r.RowId);

                if (item != null)
                    return _itemInfoCache.GetItemSources(item.Value.RowId) ?? [];

                return [];
            }
        });
    }

    public override void BuildViewFields(CompendiumViewBuilder viewBuilder, Mount row)
    {
        viewBuilder.SetupDefaults(this, row);

        var transient = _mountTransientSheet.GetRow(row.RowId);

        viewBuilder.Description =
            transient.Description.ToImGuiString();

        viewBuilder.AddTag(
            () => _unlockState.IsMountUnlocked(row) ? "Unlocked" : "未解锁",
            () => "已解锁",
            () => _unlockState.IsMountUnlocked(row) ? ImGuiColors.HealerGreen : ImGuiColors.DalamudRed);

        viewBuilder.AddTag(() => "座位 " + (row.ExtraSeats + 1), () => "该坐骑可乘坐几人？");

        var relatedItem = GetRelatedItem(row.RowId);

        if (relatedItem != null)
        {
            viewBuilder.AddSingleRowRefSection(new SingleRowRefSectionOptions()
            {
                SectionKey = "related_item",
                SectionName = "关联物品",
                RelatedRef = relatedItem.Value.AsUntypedRowRef()
            });
            var sources =
                _itemInfoCache.GetItemSources(
                    relatedItem.Value.RowId);

            viewBuilder.AddItemSourcesSection(new ItemSourcesSectionOptions()
                {
                    SectionKey = "sources",
                    SectionName = "来源",
                    Sources = sources ?? [],
                    SourceType = SourceType.Source
                });
        }

        if (row.RideBGM.RowId != 0)
        {
            viewBuilder.AddSingleRowRefSection(new SingleRowRefSectionOptions()
            {
                SectionKey = "bgm",
                SectionName = "背景音乐",
                RelatedRef = row.RideBGM.Value.AsUntypedRowRef(),
            });
        }
    }

    public override bool HasRow(uint rowId)
    {
        if (rowId == 0)
            return false;

        return _mountSheet.HasRow(rowId);
    }

    public override List<ICompendiumGrouping>? GetGroupings()
    {
        return null;
    }

    public override string Singular => "坐骑";
    public override string Plural => "坐骑";

    public override string Description =>
        "玩家可使用的坐骑。";

    public override string Key => "mounts";

    public override (string?, uint?) Icon =>
        (null, Icons.MountIcon);
}