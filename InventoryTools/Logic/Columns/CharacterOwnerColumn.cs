using System.Collections.Generic;
using System.Linq;
using CriticalCommonLib.Models;
using CriticalCommonLib.Services;
using InventoryTools.Logic.Columns.Abstract;
using InventoryTools.Services;
using Microsoft.Extensions.Logging;
using InventoryTools.Localization;

namespace InventoryTools.Logic.Columns;

public class CharacterOwnerColumn : TextColumn
{
    private readonly ICharacterMonitor _characterMonitor;
    private Dictionary<ulong, string> _characterOwners;

    public CharacterOwnerColumn(ICharacterMonitor characterMonitor, ILogger<CharacterOwnerColumn> logger, ImGuiService imGuiService) : base(logger, imGuiService)
    {
        _characterMonitor = characterMonitor;
        _characterMonitor.OnCharacterUpdated += CharacterMonitorOnOnCharacterUpdated;
        _characterOwners = new();
    }

    private void CharacterMonitorOnOnCharacterUpdated(Character? character)
    {
        _characterOwners = new();
    }

    public override string Name { get; set; } = LocalizationService.Ui(LocalizationService.Ui("Character Owner"));
    public override float Width { get; set; } = 100;

    public override string HelpText { get; set; } =
        LocalizationService.Ui(LocalizationService.Ui("Display's the name of the owner of the character this item is on if applicable(retainers, free companies, etc)"));

    public override ColumnCategory ColumnCategory { get; } = ColumnCategory.Inventory;
    public override bool HasFilter { get; set; } = true;
    public override ColumnFilterType FilterType { get; set; } = ColumnFilterType.Text;

    public override string? CurrentValue(ColumnConfiguration columnConfiguration, SearchResult searchResult)
    {
        var item = searchResult.InventoryItem;
        if (item == null)
        {
            return null;
        }

        var characterOwners = _characterOwners;
        if (characterOwners.TryGetValue(item.RetainerId, out var cached))
        {
            return cached;
        }

        Character? character = _characterMonitor.GetCharacterById(item.RetainerId);
        if (character == null)
        {
            characterOwners[item.RetainerId] = "Unknown";
            return null;
        }

        string ownerName;
        if (character.CharacterType == CharacterType.Character)
        {
            // 物品在角色背包：角色持有者 = 该角色本身
            ownerName = character.FormattedName;
        }
        else if (character.CharacterType == CharacterType.FreeCompanyChest)
        {
            // 物品在部队储物柜：角色持有者 = <部队名> + 部队内记录的角色
            var fcName = "<" + character.FormattedName + ">";
            var fcCharacters = _characterMonitor.GetFreeCompanyCharacters(character.CharacterId);
            var names = fcCharacters.Select(c => c.Value.FormattedName).ToList();
            ownerName = fcName + (names.Count > 0 ? string.Join(", ", names) : "");
        }
        else
        {
            // 物品在雇员/部队等库存：角色持有者 = 所属角色
            var mainOwner = _characterMonitor.GetCharacterById(character.OwnerId);
            ownerName = mainOwner?.FormattedName ?? character.FormattedName;
        }

        characterOwners[item.RetainerId] = ownerName;
        return ownerName;
    }

    public override void Dispose()
    {
        _characterMonitor.OnCharacterUpdated -= CharacterMonitorOnOnCharacterUpdated;
        base.Dispose();
    }
}