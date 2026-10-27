using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace InventoryTools.Host;

/// <summary>
/// 插件内部名从 InventoryTools 改为 InventoryToolsZh，以避开 Dalamud 的
/// 「官方仓库同名插件禁止第三方源替换」限制。首次加载时把旧配置和
/// 历史数据复制到新名字下（只复制、不移动，旧数据保持原样）。
/// </summary>
internal static class LegacyConfigMigration
{
    [ModuleInitializer]
    internal static void Migrate()
    {
        try
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrEmpty(appData))
            {
                return;
            }

            foreach (var launcher in new[] { "XIVLauncherCN", "XIVLauncher" })
            {
                var root = Path.Combine(appData, launcher, "pluginConfigs");
                if (!Directory.Exists(root))
                {
                    continue;
                }

                CopyFileIfMissing(
                    Path.Combine(root, "InventoryTools.json"),
                    Path.Combine(root, "InventoryToolsZh.json"));

                CopyDirectoryIfMissing(
                    Path.Combine(root, "InventoryTools"),
                    Path.Combine(root, "InventoryToolsZh"));
            }
        }
        catch
        {
            // 迁移失败不能阻止插件加载。
        }
    }

    private static void CopyFileIfMissing(string source, string target)
    {
        if (!File.Exists(source) || File.Exists(target))
        {
            return;
        }

        Directory.CreateDirectory(Path.GetDirectoryName(target)!);
        File.Copy(source, target, overwrite: false);
    }

    private static void CopyDirectoryIfMissing(string source, string target)
    {
        if (!Directory.Exists(source) || Directory.Exists(target))
        {
            return;
        }

        var temp = target + ".migrating";
        try
        {
            if (Directory.Exists(temp))
            {
                Directory.Delete(temp, true);
            }

            CopyDirectory(source, temp);
            Directory.Move(temp, target);
        }
        catch
        {
            try
            {
                if (Directory.Exists(temp))
                {
                    Directory.Delete(temp, true);
                }
            }
            catch
            {
                // ignore
            }
        }
    }

    private static void CopyDirectory(string source, string target)
    {
        Directory.CreateDirectory(target);

        foreach (var dir in Directory.GetDirectories(source, "*", SearchOption.AllDirectories))
        {
            Directory.CreateDirectory(dir.Replace(source, target));
        }

        foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
        {
            var destination = file.Replace(source, target);
            Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
            File.Copy(file, destination, overwrite: false);
        }
    }
}
