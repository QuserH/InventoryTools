using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Model;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.GameSheets.Sheets.Rows;
using AllaganLib.Shared.Extensions;
using AllaganLib.Shared.Misc;
using CriticalCommonLib.Crafting;
using CriticalCommonLib.Services;
using DalaMock.Host.Mediator;
using Dalamud.Game.Text;
using Dalamud.Interface.Colors;
using InventoryTools.Compendium.Interfaces;
using InventoryTools.Compendium.Models;
using InventoryTools.Compendium.Sections;
using InventoryTools.Compendium.Sections.Options;
using InventoryTools.Compendium.Services;
using InventoryTools.Ui;
using Lumina.Excel;
using Lumina.Excel.Sheets;
using InventoryTools.Localization;

namespace InventoryTools.Compendium.Types;

public class ItemCompendiumType : CompendiumType<ItemRow>
{
    private readonly ItemSheet _itemSheet;
    private readonly IUnlockTrackerService _unlockTrackerService;
    private readonly IItemObtainabilityService _obtainabilityService;

    public ItemCompendiumType(ItemSheet itemSheet, CompendiumTable<ItemRow>.Factory tableFactory, CompendiumColumnBuilder<ItemRow>.Factory columnBuilder, CompendiumViewBuilder.Factory viewBuilderFactory, IUnlockTrackerService unlockTrackerService, IItemObtainabilityService obtainabilityService) : base(tableFactory, columnBuilder, viewBuilderFactory)
    {
        _itemSheet = itemSheet;
        _unlockTrackerService = unlockTrackerService;
        _obtainabilityService = obtainabilityService;
    }

    public override ICompendiumTable<WindowState, MessageBase> BuildTable()
    {
        return Factory.Invoke(new CompendiumTableOptions<ItemRow>()
        {
            Columns = BuiltColumns,
            CompendiumType = this,
            Key = "items",
            Name = LocalizationService.Ui("Items")
        });
    }

    public override string? GetName(ItemRow row)
    {
        return row.NameString;
    }

    public override string? GetSubtitle(ItemRow row)
    {
        return row.Base.ItemSearchCategory.ValueNullable?.Name.ToImGuiString();
    }

    public override (string?, uint?) GetIcon(ItemRow row)
    {
        return (null, row.Icon);
    }

    public override uint GetRowId(ItemRow row)
    {
        return row.RowId;
    }

    public override ItemRow? GetRow(uint row)
    {
        if (row == 0)
        {
            return null;
        }
        return _itemSheet.GetRow(row);
    }

    public override List<ItemRow> GetRows()
    {
        return _itemSheet.Where(c => c.NameString != string.Empty).ToList();
    }

    public override void BuildColumns(CompendiumColumnBuilder<ItemRow> builder)
    {
        builder.AddCompendiumOpenViewColumn(new() { Key = "icon", Name = LocalizationService.Ui("##Icon"), HelpText = LocalizationService.Ui("The icon of the leve"), Version = "14.0.3", ValueSelector = this.GetIcon, CompendiumType = this, RowIdSelector = row => row.RowId });
        builder.AddStringColumn(new() { Key = "name", Name = LocalizationService.Ui("Name"), HelpText = LocalizationService.Ui("The name of the leve"), Version = "14.0.3", ValueSelector = row => row.NameString });
    }

    public override void BuildViewFields(CompendiumViewBuilder viewBuilder, ItemRow row)
    {
        viewBuilder.SetupDefaults(this, row);
        viewBuilder.Description = row.Base.Description.ToImGuiString();
        viewBuilder.AddTag(() => LocalizationService.Ui("iLvl ") + row.Base.LevelItem.RowId, () => LocalizationService.Ui("The item level of the item"));
        viewBuilder.AddTag(() => LocalizationService.Ui("Patch ") + row.Patch, () => LocalizationService.Ui("The patch the item was introduced"));
        if (row.CanBeAcquired)
        {
            viewBuilder.AddTag(
                () =>
                {
                    var isUnlocked = _unlockTrackerService.IsUnlocked(row);
                    if (isUnlocked == null) return LocalizationService.Ui("Acquired?");
                    return isUnlocked.Value ? "Acquired" : LocalizationService.Ui("Not Acquired");
                },
                () => LocalizationService.Ui("Is the item acquired?"),
                () =>
                {
                    var isUnlocked = _unlockTrackerService.IsUnlocked(row);
                    if (isUnlocked == null) return ImGuiColors.DalamudYellow;
                    return isUnlocked.Value ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed;
                });
        }

        if (row.CanBeCrafted)
        {
            viewBuilder.AddTag(() => "Craftable", () => LocalizationService.Ui("Is the item craftable?"));
        }
        if (row.CanBeDesynthed)
        {
            viewBuilder.AddTag(() => "Desynthable", () => LocalizationService.Ui("Can the item be desynthed?"));
        }

        viewBuilder.AddItemSourcesSection(new ItemSourcesSectionOptions()
        {
            Sources = row.Sources,
            SourceType = SourceType.Source,
            SectionKey = "sources",
            SectionName = LocalizationService.Ui("Sources"),
        });

        viewBuilder.AddItemSourcesSection(new ItemSourcesSectionOptions()
        {
            Sources = row.Uses,
            SourceType = SourceType.Use,
            SectionKey = "uses",
            SectionName = LocalizationService.Ui("Uses")
        });

        viewBuilder.AddMetadataSection(new MetadataSectionOptions()
        {
            SectionKey = "information",
            SectionName = LocalizationService.Ui("Information"),
            Rows = new List<MetadataSectionOptions.Row>()
            {
                new()
                {
                    Label = LocalizationService.Ui("Buy from Vendor Price"),
                    Value = () => row.BuyFromVendorPrice + SeIconChar.Gil.ToIconString(),
                    ShouldDraw = () => row.BuyFromVendorPrice != 0 && row.HasSourcesByType(ItemInfoType.GilShop)
                },
                new()
                {
                    Label = LocalizationService.Ui("Sell to Vendor Price"),
                    Value = () => row.SellToVendorPrice + SeIconChar.Gil.ToIconString(),
                    ShouldDraw = () => row.SellToVendorPrice != 0
                },
            }
        });
        viewBuilder.AddSingleRowRefSection(new SingleRowRefSectionOptions()
        {
            SectionKey = "desynthesis_class",
            SectionName = LocalizationService.Ui("Desynthesis Class"),
            RelatedRef = (RowRef)row.Base.ClassJobRepair,
            HideWhenEmpty = true
        });
        var sharedModels = row.GetSharedModels();
        viewBuilder.AddItemListSection(new ItemListSectionOptions()
        {
            SectionKey = "shared_models",
            SectionName = LocalizationService.Ui("Shared Models"),
            Items = sharedModels.Select(c => new ItemInfo(c)),
            HideWhenEmpty = true
        });

        var (allRequirements, requirementRows) = BuildObtainabilityRows(row);
        if (allRequirements.Count > 0)
        {
            viewBuilder.AddTag(
                () => allRequirements.All(r => r.IsMet) ? "Unlocked" : LocalizationService.Ui("Not Unlocked"),
                () => LocalizationService.Ui("Are all unlock requirements met for this item?"),
                () => allRequirements.All(r => r.IsMet) ? ImGuiColors.ParsedGreen : ImGuiColors.DalamudRed);
        }

        viewBuilder.AddMetadataSection(new MetadataSectionOptions()
        {
            SectionKey = "unlock_requirements",
            SectionName = LocalizationService.Ui("Unlock Requirements"),
            Rows = requirementRows,
            HideWhenEmpty = true
        });

        viewBuilder.AddLink($"https://www.garlandtools.org/db/#item/{row.GarlandToolsId}", "Open in Garland Tools", "garlandtools");
        viewBuilder.AddLink($"https://ffxivteamcraft.com/db/en/item/{row.RowId}", "Open in Teamcraft", "teamcraft");
        if (row.CanBePlacedOnMarket)
        {
            viewBuilder.AddLink($"https://universalis.app/market/{row.RowId}", "Open in Universalis", "universalis");
        }

        viewBuilder.AddLink($"https://ffxiv.gamerescape.com/wiki/{HttpUtility.UrlEncode(row.GamerEscapeName)}?useskin=Vector", LocalizationService.Ui("Open in Gamerescape"), "gamerescape");
        viewBuilder.AddLink($"https://ffxiv.consolegameswiki.com/wiki/{HttpUtility.UrlEncode(row.ConsoleGamesWikiName)}", "Open in Console Games Wiki", "consolegameswiki");
    }

    private (List<ObtainabilityRequirement> AllRequirements, List<MetadataSectionOptions.Row> Rows) BuildObtainabilityRows(ItemRow row)
    {
        var allRequirements = new List<ObtainabilityRequirement>();
        var rows = new List<MetadataSectionOptions.Row>();

        var sourcesToCheck = new (IngredientPreferenceType Type, string Label)[]
        {
            (IngredientPreferenceType.Crafting,    LocalizationService.Ui("Crafting")),
            (IngredientPreferenceType.Mining,      LocalizationService.Ui("Mining")),
            (IngredientPreferenceType.Botany,      LocalizationService.Ui("Botany")),
            (IngredientPreferenceType.Fishing,     LocalizationService.Ui("Fishing")),
            (IngredientPreferenceType.SpearFishing,LocalizationService.Ui("Spearfishing")),
        };

        foreach (var (preferenceType, label) in sourcesToCheck)
        {
            RecipeRow? recipe = null;
            if (preferenceType == IngredientPreferenceType.Crafting)
            {
                recipe = row.Sources.OfType<ItemCraftResultSource>().FirstOrDefault()?.Recipe;
                if (recipe == null) continue;
            }

            var requirements = _obtainabilityService.GetRequirements(row, preferenceType, recipe);
            foreach (var req in requirements)
            {
                allRequirements.Add(req);
                var captured = req;
                rows.Add(new MetadataSectionOptions.Row
                {
                    Label = $"{label}: {captured.Description}",
                    Value = () => captured.IsMet ? LocalizationService.Ui("Met") : LocalizationService.Ui("Not Met"),
                });
            }
        }

        return (allRequirements, rows);
    }

    public override bool HasRow(uint rowId)
    {
        if (rowId == 0)
        {
            return false;
        }
        return _itemSheet.GetRowOrDefault(rowId) != null;
    }

    public override List<Type>? RelatedTypes => [typeof(Item)];

    public override bool ShowInListing => false;
    public override Type ViewRedirection => typeof(ItemWindow);

    public override string Singular => LocalizationService.Ui("Item");
    public override string Plural => LocalizationService.Ui("Items");
    public override string Description => LocalizationService.Ui("All the items available in the game");
    public override string Key => "items";
    public override (string?, uint?) Icon => (null, Icons.QuestionMarkBag);
}