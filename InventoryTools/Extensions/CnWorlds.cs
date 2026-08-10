using System.Collections.Generic;

namespace InventoryTools.Extensions;

/// <summary>
/// 国服（中国区）服务器信息。世界 ID 与中文名来源：Universalis /api/v2/data-centers 与 /api/v2/worlds 中国区数据。
/// 顺序：豆豆柴 → 猫小胖 → 莫古力 → 陆行鸟。
/// </summary>
public static class CnWorlds
{
    /// <summary>大区名 → 该大区所有世界 ID（保持此顺序）。</summary>
    public static readonly Dictionary<string, List<uint>> DataCenters = new()
    {
        ["豆豆柴"] = new() { 1201, 1183, 1180, 1192, 1186 },
        ["猫小胖"] = new() { 1043, 1169, 1106, 1045, 1177, 1178, 1179 },
        ["莫古力"] = new() { 1172, 1076, 1171, 1170, 1113, 1121, 1166, 1176 },
        ["陆行鸟"] = new() { 1167, 1081, 1042, 1044, 1060, 1173, 1174, 1175 },
    };

    /// <summary>世界 ID → 中文名（仅在 BuildNames 内使用，需先于 Names 初始化）。</summary>
    private static readonly Dictionary<uint, string> WorldNames = new()
    {
        // 豆豆柴
        [1201] = "红茶川",
        [1183] = "银泪湖",
        [1180] = "太阳海岸",
        [1192] = "水晶塔",
        [1186] = "伊修加德",
        // 猫小胖
        [1043] = "紫水栈桥",
        [1169] = "延夏",
        [1106] = "静语庄园",
        [1045] = "摩杜纳",
        [1177] = "海猫茶屋",
        [1178] = "柔风海湾",
        [1179] = "琥珀原",
        // 莫古力
        [1172] = "白银乡",
        [1076] = "白金幻象",
        [1171] = "神拳痕",
        [1170] = "潮风亭",
        [1113] = "旅人栈桥",
        [1121] = "拂晓之间",
        [1166] = "龙巢神殿",
        [1176] = "梦羽宝境",
        // 陆行鸟
        [1167] = "红玉海",
        [1081] = "神意之地",
        [1042] = "拉诺西亚",
        [1044] = "幻影群岛",
        [1060] = "萌芽池",
        [1173] = "宇宙和音",
        [1174] = "沃仙曦染",
        [1175] = "晨曦王座",
    };

    /// <summary>世界 ID → (中文名, 大区名)。</summary>
    public static readonly Dictionary<uint, (string World, string DataCenter)> Names = BuildNames();

    /// <summary>全部国服世界 ID（按大区顺序）。</summary>
    public static List<uint> AllWorlds { get; } = BuildAllWorlds();

    /// <summary>判断是否为国服世界。</summary>
    public static bool IsCnWorld(uint worldId) => Names.ContainsKey(worldId);

    private static Dictionary<uint, (string World, string DataCenter)> BuildNames()
    {
        var dict = new Dictionary<uint, (string, string)>();
        foreach (var dc in DataCenters)
        {
            foreach (var worldId in dc.Value)
            {
                dict[worldId] = (WorldNames[worldId], dc.Key);
            }
        }

        return dict;
    }

    private static List<uint> BuildAllWorlds()
    {
        var list = new List<uint>();
        foreach (var dc in DataCenters)
        {
            list.AddRange(dc.Value);
        }

        return list;
    }
}
