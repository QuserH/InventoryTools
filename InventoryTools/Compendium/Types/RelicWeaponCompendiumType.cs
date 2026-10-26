using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.Shared.Extensions;
using AllaganLib.Shared.Misc;
using DalaMock.Host.Mediator;
using Dalamud.Utility;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Models;
using InventoryTools.Compendium.Sections;
using InventoryTools.Compendium.Sections.Options;
using InventoryTools.Compendium.Services;
using InventoryTools.Compendium.Types.Extra;
using InventoryTools.Localizers;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using LuminaSupplemental.Excel.Model;
using InventoryTools.Localization;

namespace InventoryTools.Compendium.Types;

public class RelicWeaponCompendiumType : CompendiumType<RelicWeaponGroup>
{
    private readonly ExcelSheet<ClassJob> _classJobSheet;
    private readonly ItemSheet _itemSheet;
    private readonly ILocalizer<RelicWeaponType> _weaponTypeLocalizer;
    private readonly ILocalizer<RelicWeaponCategory> _weaponCategoryLocalizer;
    private readonly Lazy<List<RelicWeaponGroup>> _groupedWeapons;

    public RelicWeaponCompendiumType(List<RelicWeapon> relicWeapons, ExcelSheet<ClassJob> classJobSheet, ItemSheet itemSheet, ILocalizer<RelicWeaponType> weaponTypeLocalizer, ILocalizer<RelicWeaponCategory> weaponCategoryLocalizer, CompendiumTable<RelicWeaponGroup>.Factory tableFactory, CompendiumColumnBuilder<RelicWeaponGroup>.Factory columnBuilder, CompendiumViewBuilder.Factory viewBuilderFactory) : base(tableFactory, columnBuilder, viewBuilderFactory)
    {
        _classJobSheet = classJobSheet;
        _itemSheet = itemSheet;
        _weaponTypeLocalizer = weaponTypeLocalizer;
        _weaponCategoryLocalizer = weaponCategoryLocalizer;
        _groupedWeapons = new Lazy<List<RelicWeaponGroup>>(() =>
        {
            uint rowId = 0;
            return relicWeapons.GroupBy(c => (c.Category, c.ClassJob.RowId))
                .Select(c => new RelicWeaponGroup(c.First().ClassJob, c.Key.Category, rowId++, c.ToList())).ToList();
        }, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public override ICompendiumTable<WindowState, MessageBase> BuildTable()
    {
        return Factory.Invoke(new CompendiumTableOptions<RelicWeaponGroup>()
        {
            Columns = BuiltColumns,
            CompendiumType = this,
            Key = "relic_weapons",
            Name = "特殊武器（战斗）",
        });
    }

    public override string? GetName(RelicWeaponGroup row)
    {
        return row.ClassJob.Value.Name.ToImGuiString().FirstCharToUpper() + " - " + _weaponCategoryLocalizer.Format(row.WeaponCategory); //TODO: Add formatter
    }

    public override string? GetSubtitle(RelicWeaponGroup row)
    {
        return null; //todo: add formatter
    }

    public override (string?, uint?) GetIcon(RelicWeaponGroup row)
    {
        return (null, Icons.WeaponIcon); //come up with better icons
    }

    public override uint GetRowId(RelicWeaponGroup row)
    {
        return row.RowId;
    }

    public override RelicWeaponGroup? GetRow(uint row)
    {
        return _groupedWeapons.Value[(int)row];
    }

    public override List<RelicWeaponGroup> GetRows()
    {
        return _groupedWeapons.Value;
    }

    public override void BuildColumns(CompendiumColumnBuilder<RelicWeaponGroup> builder)
    {
        builder.AddCompendiumOpenViewColumn(new(){Key = "icon", Name = "##Icon", HelpText = "该背景音乐的图标", Version = "14.1.3", ValueSelector = this.GetIcon, CompendiumType = this, RowIdSelector = row => row.RowId});

        var grouping = builder.CompendiumGrouping?.Key;
        if (grouping != "category")
        {
            builder.AddStringColumn(new ()
            {
                ValueSelector = row => _weaponCategoryLocalizer.Format(row.WeaponCategory),
                Name = "分类",
                Key = "Category",
                HelpText = "物品的分类",
                Version = "14.1.3"
            });
        }
        if (grouping != "class_job")
        {
            builder.AddStringColumn(new ()
            {
                ValueSelector = row => row.ClassJob.Value.Name.ToImGuiString().FirstCharToUpper() ?? "未知",
                Name = "职业",
                Key = "class_job",
                HelpText = "物品的职业",
                Version = "14.1.3"
            });
        }
        if (builder.CompendiumGrouping != null && builder.GroupItem != null)
        {
            if (builder.CompendiumGrouping.Key == "category")
            {
                if (builder.GroupItem is RelicWeaponCategory weaponCategory)
                {
                    RelicWeaponType relicWeaponType;
                    switch (weaponCategory)
                    {
                        case RelicWeaponCategory.Zodiac:
                            relicWeaponType = RelicWeaponType.ZodiacBase;
                            break;
                        case RelicWeaponCategory.Anima:
                            relicWeaponType = RelicWeaponType.AnimaAnimated;
                            break;
                        case RelicWeaponCategory.Eurekan:
                            relicWeaponType = RelicWeaponType.EurekanAntiquated;
                            break;
                        case RelicWeaponCategory.Resistance:
                            relicWeaponType = RelicWeaponType.ResistanceResistance;
                            break;
                        case RelicWeaponCategory.Manderville:
                            relicWeaponType = RelicWeaponType.MandervilleManderville;
                            break;
                        case RelicWeaponCategory.Phantom:
                            relicWeaponType = RelicWeaponType.PhantomPenumbrae;
                            break;
                        default:
                            return;
                    }

                    var maxItems = _groupedWeapons.Value.Where(c => c.WeaponCategory == weaponCategory).Max(c => c.RelicWeapons.Count);
                    for (int i = 0; i < maxItems; i++)
                    {
                        var i1 = i;
                        builder.AddItemsColumn(new()
                        {
                            Key = "item_" + i,
                            Name = _weaponTypeLocalizer.Format(relicWeaponType), HelpText = "形态 " + (i + 1) + "的此武器。",
                            Version = "14.1.3",
                            ValueSelector = relicWeapon => weaponCategory == relicWeapon.WeaponCategory ? [_itemSheet.GetRow(relicWeapon.RelicWeapons[i1].ItemId), _itemSheet.GetRow(relicWeapon.RelicWeapons[i1].OffhandItemId)] : []
                        });
                        relicWeaponType++;
                    }
                }


                return;
            }
        }
        for (int i = 0; i < _groupedWeapons.Value.Max(c => c.RelicWeapons.Count); i++)
        {
            var i1 = i;
            builder.AddItemColumn(new()
            {
                Key = "item_" + i,
                Name = "形态 " + (i + 1), HelpText = "形态 " + (i + 1) + "的此武器。",
                Version = "14.1.3",
                ValueSelector = relicWeapon =>
                    i1 >= 0 && i1 < relicWeapon.RelicWeapons.Count ? relicWeapon.RelicWeapons[i1].ItemId : null
            });
        }
    }

    public override void BuildViewFields(CompendiumViewBuilder viewBuilder, RelicWeaponGroup row)
    {
        viewBuilder.SetupDefaults(this, row);
        //Maybe need to work on groups or some sort of item display that displays each item in a table the form and an arrow to the next
        List<ItemFlowEntry> itemFlowEntries = [];
        for (var index = 0; index < row.RelicWeapons.Count; index++)
        {
            var relicWeapon = row.RelicWeapons[index];
            itemFlowEntries.Add(new ItemFlowEntry()
            {
                Item = _itemSheet.GetRow(relicWeapon.ItemId),
                Item2 = relicWeapon.OffhandItemId == 0 ? null : _itemSheet.GetRowOrDefault(relicWeapon.OffhandItemId),
                Title = (index + 1) + ". " + _weaponTypeLocalizer.Format(relicWeapon.Type)
            });
        }

        viewBuilder.AddSingleRowRefSection(new SingleRowRefSectionOptions()
        {
            SectionKey = "class_job",
            SectionName = "职业",
            RelatedRef = (RowRef)row.ClassJob
        });
        viewBuilder.AddCollectionRowRefSection(new CollectionRowRefSectionOptions()
        {
            SectionKey = "related_quests",
            SectionName = "相关任务",
            RelatedRefs = row.Quests.Where(c => c.RowId != 0).Select(c => (RowRef)c).ToList(),
        });
        viewBuilder.AddItemFlowSection(new ItemFlowSectionOptions()
        {
            SectionKey = "weapons",
            SectionName = "武器",
            Items = itemFlowEntries,
            ItemsPerColumn = Math.Max(3, (int)Math.Ceiling((double)itemFlowEntries.Count / 3))
        });
    }

    public override bool HasRow(uint rowId)
    {
        return (int)rowId >= 0 && (int)rowId < _groupedWeapons.Value.Count;
    }

    public override List<ICompendiumGrouping>? GetGroupings()
    {
        return new List<ICompendiumGrouping>()
        {
            new CompendiumGrouping<RelicWeaponGroup>()
            {
                Key = "class_job",
                Name = "职业",
                GroupFunc = row => row.ClassJob.RowId,
                GroupMapping = row =>
                {
                    var classJobId = (uint)row;
                    var name = _classJobSheet.GetRowOrDefault(classJobId)?.Name.ToImGuiString().FirstCharToUpper() ??
                               "无";
                    if (name == string.Empty)
                    {
                        name = "不分组";
                    }

                    return name;
                }
            },
            new CompendiumGrouping<RelicWeaponGroup>()
            {
                Key = "category",
                Name = "分类",
                GroupFunc = row => row.WeaponCategory,
                GroupMapping = row =>
                {
                    var relicWeaponCategory = (RelicWeaponCategory)row;
                    return _weaponCategoryLocalizer.Format(relicWeaponCategory);
                }
            }
        };
    }

    public override List<Type>? RelatedTypes => [typeof(RelicWeapon)];

    public override uint? RemapType(Type type, uint rowId)
    {
        if (type == typeof(RelicWeapon))
        {
            return _groupedWeapons.Value.FirstOrDefault(c => c.RelicWeapons.Any(d => d.RowId == rowId))?.RowId;
        }

        return null;
    }

    public override string? GetDefaultGrouping()
    {
        return "category";
    }

    public override string Singular => "特殊武器（战斗）";
    public override string Plural => "特殊武器（战斗）";
    public override string Description => "特殊武器（战斗）";
    public override string Key => "relic_weapons";
    public override (string?, uint?) Icon => (null, Icons.WeaponIcon);
}
