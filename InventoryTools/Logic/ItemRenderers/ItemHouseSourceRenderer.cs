using System;
using System.Collections.Generic;
using AllaganLib.GameSheets.Caches;
using AllaganLib.GameSheets.ItemSources;
using AllaganLib.GameSheets.Sheets;
using AllaganLib.Shared.Time;
using CriticalCommonLib.Models;
using Dalamud.Interface.Colors;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using Dalamud.Bindings.ImGui;
using InventoryTools.Localization;

namespace InventoryTools.Logic.ItemRenderers;

public abstract class ItemHouseSourceRenderer<T> : ItemInfoRenderer<T> where T : ItemHouseSource
{
    private readonly ItemInfoType _type;

    public override IReadOnlyList<ItemInfoRenderCategory>? Categories => [ItemInfoRenderCategory.House];

    public ItemHouseSourceRenderer(ItemInfoType type, ItemSheet itemSheet, MapSheet mapSheet,
        ITextureProvider textureProvider, IDalamudPluginInterface dalamudPluginInterface) : base(textureProvider, dalamudPluginInterface, itemSheet, mapSheet)
    {
        _type = type;
    }

    public override RendererType RendererType => RendererType.Use;
    public override ItemInfoType Type => _type;
    public override bool ShouldGroup => true;

    public override Action<ItemSource> DrawTooltip => source =>
    {
        var asSource = (ItemHouseSource)source;
        var setName = asSource.HousingPreset.Value.Singular.ExtractText();
        if (setName == string.Empty)
        {
            ImGui.Text("任何房屋中均不默认放置。");
        }
        else
        {
            ImGui.Text("默认显示于" + setName);
        }
    };

    public override Func<ItemSource, string> GetName => source =>
    {
        var asSource = (ItemHouseSource)source;
        return asSource.Item.NameString;
    };
    public override Func<ItemSource, int> GetIcon => _ =>
    {
        //TODO: come up with an icon for each
        return Icons.RedXIcon;
    };

    public override Func<ItemSource, string> GetDescription => source =>
    {
        var asSource = AsSource(source);
        var setName = asSource.HousingPreset.Value.Singular.ExtractText();
        if (setName == string.Empty)
        {
           return "任何房屋中均不默认放置。";
        }
        else
        {
            return "默认显示于" + setName;
        }
    };
}

public class ItemHouseDoorSourceRenderer : ItemHouseSourceRenderer<ItemHouseDoorSource>
{
    public ItemHouseDoorSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(ItemInfoType.HouseDoor, itemSheet, mapSheet, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "房屋设施（门）";

    public override string HelpText => "该物品是否可放置在房屋的门框栏位？";
}


public class ItemHouseFlooringSourceRenderer : ItemHouseSourceRenderer<ItemHouseFlooringSource>
{
    public ItemHouseFlooringSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(ItemInfoType.HouseFlooring, itemSheet, mapSheet, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "房屋设施（地板）";
    public override string HelpText => "该物品是否可放置在房屋的地面栏位？";
}

public class ItemHouseLightingSourceRenderer : ItemHouseSourceRenderer<ItemHouseLightingSource>
{
    public ItemHouseLightingSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(ItemInfoType.HouseLighting, itemSheet, mapSheet, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "房屋设施（照明）";
    public override string HelpText => "该物品是否可放置在房屋的照明栏位？";
}

public class ItemHouseRoofSourceRenderer : ItemHouseSourceRenderer<ItemHouseRoofSource>
{
    public ItemHouseRoofSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(ItemInfoType.HouseRoof, itemSheet, mapSheet, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "房屋设施（屋顶）";
    public override string HelpText => "该物品是否可放置在房屋的屋顶栏位？";
}

public class ItemHouseWallpaperSourceRenderer : ItemHouseSourceRenderer<ItemHouseWallpaperSource>
{
    public ItemHouseWallpaperSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(ItemInfoType.HouseWallpaper, itemSheet, mapSheet, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "房屋设施（壁纸）";
    public override string HelpText => "该物品是否可放置在房屋的室内墙壁栏位？";
}

public class ItemHouseWallSourceRenderer : ItemHouseSourceRenderer<ItemHouseWallSource>
{
    public ItemHouseWallSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(ItemInfoType.HouseWall, itemSheet, mapSheet, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "房屋设施（墙壁）";
    public override string HelpText => "该物品是否可放置在房屋的室外墙壁栏位？";
}

public class ItemHouseWindowSourceRenderer : ItemHouseSourceRenderer<ItemHouseWindowSource>
{
    public ItemHouseWindowSourceRenderer(ItemSheet itemSheet, MapSheet mapSheet, ITextureProvider textureProvider,
        IDalamudPluginInterface dalamudPluginInterface) : base(ItemInfoType.HouseWindow, itemSheet, mapSheet, textureProvider, dalamudPluginInterface)
    {
    }

    public override string SingularName => "房屋设施（窗户）";
    public override string HelpText => "该物品是否可放置在房屋的窗户栏位？";
}
