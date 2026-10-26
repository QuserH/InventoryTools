using System;
using System.Collections.Generic;
using System.Linq;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.Model;
using DalaMock.Host.Mediator;
using Dalamud.Bindings.ImGui;
using Humanizer;
using InventoryTools.Compendium.Columns.Options;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Models;
using InventoryTools.Compendium.Sections;
using InventoryTools.Compendium.Sections.Options;
using InventoryTools.Compendium.Services;
using OtterGui.Extensions;
using Icons = AllaganLib.Shared.Misc.Icons;
using InventoryTools.Localization;

namespace InventoryTools.Compendium.Types;

public class SharedModelCompendiumType : CompendiumType<SharedModelCache.SharedModelGroup>
{
    private readonly SharedModelCache _sharedModelCache;

    public SharedModelCompendiumType(CompendiumTable<SharedModelCache.SharedModelGroup>.Factory tableFactory, CompendiumColumnBuilder<SharedModelCache.SharedModelGroup>.Factory columnBuilder, CompendiumViewBuilder.Factory viewBuilderFactory, SharedModelCache sharedModelCache) : base(tableFactory, columnBuilder, viewBuilderFactory)
    {
        _sharedModelCache = sharedModelCache;
    }

    public override ICompendiumTable<WindowState, MessageBase> BuildTable()
    {
        return Factory.Invoke(new()
        {
            Key = "shared_models",
            Name = Plural,
            Columns = BuiltColumns,
            CompendiumType = this,
            BuildContextMenu = null
        });
    }

    public override string? GetName(SharedModelCache.SharedModelGroup row)
    {
        return "共享模型 #" + _sharedModelCache.IndexOf(row);
    }

    public override string? GetSubtitle(SharedModelCache.SharedModelGroup row)
    {
        return row.Items.Count + " 件物品";
    }

    public override (string?, uint?) GetIcon(SharedModelCache.SharedModelGroup row)
    {
        return (null, row.Items.First().Icon);
    }

    public override uint GetRowId(SharedModelCache.SharedModelGroup row)
    {
        return (uint)GetRows().IndexOf(row);
    }

    public override SharedModelCache.SharedModelGroup GetRow(uint row)
    {
        return _sharedModelCache[(int)row];
    }

    public override bool HasRow(uint rowId)
    {
        return (int)rowId >= 0 && (int)rowId < _sharedModelCache.Count;
    }

    public override List<SharedModelCache.SharedModelGroup> GetRows()
    {
        return _sharedModelCache.ToList();
    }

    public override void BuildColumns(CompendiumColumnBuilder<SharedModelCache.SharedModelGroup> builder)
    {
        builder.AddCompendiumOpenViewColumn(new(){Key = "icon", Name = "##Icon", HelpText = "该共享模型的图标", Version = "14.0.3", ValueSelector = row => ("armor", null), CompendiumType = this, RowIdSelector = row => (uint)_sharedModelCache.IndexOf(row)});
        builder.AddStringColumn(new StringColumnOptions<SharedModelCache.SharedModelGroup>
        {
            ValueSelector = row => row.Items.First().ClassJobCategory?.Base.Name.ExtractText() ?? "未知",
            Name = "职业",
            Key = "class_job",
            HelpText = "物品的职业",
            Version = "14.0.3"
        });
        builder.AddStringColumn(new StringColumnOptions<SharedModelCache.SharedModelGroup>
        {
            ValueSelector = row => string.Join(", ", row.Items.First().EquipSlotCategory?.PossibleSlots.Select(EquipSlotName) ?? []),
            Name = "装备栏位",
            Key = "equip_slots",
            HelpText = "物品的装备栏位",
            Version = "14.0.3"
        });
        builder.AddItemsColumn(new ItemsColumnOptions<SharedModelCache.SharedModelGroup>
        {
            ValueSelector = row => row.Items.ToList(),
            Name = "物品",
            Key = "items",
            HelpText = "共享此模型的物品",
            Version = "14.0.3",
            ColumnFlags = ImGuiTableColumnFlags.WidthStretch
        });
    }

    public override void BuildViewFields(CompendiumViewBuilder viewBuilder, SharedModelCache.SharedModelGroup row)
    {
        viewBuilder.Icon = row.Items.First().Icon;
        viewBuilder.Title = "共享模型 #" + _sharedModelCache.IndexOf(row);
        viewBuilder.Subtitle = row.Items.Count + " 件物品";
        viewBuilder.AddItemListSection(new ItemListSectionOptions()
        {
            Items = row.Items.Select(c => new ItemInfo(c)),
            SectionKey = "items",
            SectionName = "物品",
        });
    }

    public override string Singular => "共享模型套装";
    public override string Plural => "共享模型套装";
    public override string Description => "共享同一模型的物品。";
    public override string Key => "shared_models";
    public override (string?, uint?) Icon => (null, Icons.CombinedClothingIcon);

    private static string EquipSlotName(EquipSlot slot) => slot switch
    {
        EquipSlot.MainHand => "主手",
        EquipSlot.OffHand => "副手",
        EquipSlot.Head => "头部",
        EquipSlot.Body => "身体",
        EquipSlot.Gloves => "手部",
        EquipSlot.Legs => "腿部",
        EquipSlot.Feet => "脚部",
        EquipSlot.Ears => "耳部",
        EquipSlot.Neck => "颈部",
        EquipSlot.Wrists => "腕部",
        EquipSlot.FingerR => "右手戒指",
        EquipSlot.FingerL => "左手戒指",
        EquipSlot.SoulCrystal => "特职水晶",
        _ => LocalizationService.Ui(slot.Humanize()),
    };
}
