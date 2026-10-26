using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using AllaganLib.Shared.Interfaces;
using AllaganLib.Shared.Services;
using CriticalCommonLib.Interfaces;
using CriticalCommonLib.Models;
using CriticalCommonLib.Resolvers;
using Dalamud.Plugin.Services;
using InventoryTools.Converters;
using LuminaSupplemental.Excel.Model;
using LuminaSupplemental.Excel.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Task = System.Threading.Tasks.Task;
using InventoryTools.Localization;

namespace InventoryTools.Services
{
    using Dalamud.Plugin;

    public class ConfigurationManagerService : BackgroundService
    {
        public ILogger<ConfigurationManagerService> Logger { get; }

        public delegate void ConfigurationChangedDelegate();
        private readonly IFramework _framework;
        private bool _configurationLoaded = false;
        private Task? _currentInventorySaveTask = null;

        public event ConfigurationChangedDelegate? ConfigurationChanged;

        public ConfigurationManagerService(IFramework framework, IDalamudPluginInterface pluginInterfaceService, ILogger<ConfigurationManagerService> logger, BackgroundTaskQueue.Factory taskQueueFactory, MinifyResolver minifyResolver, ContainerAwareCsvLoader containerAwareCsvLoader)
        {
            Logger = logger;
            _pluginInterfaceService = pluginInterfaceService;
            _saveQueue = taskQueueFactory.Invoke("配置保存队列");
            _minifyResolver = minifyResolver;
            _containerAwareCsvLoader = containerAwareCsvLoader;
            _framework = framework;
            _framework.Update += OnUpdate;
        }

        private void OnUpdate(IFramework framework)
        {
            if (_configurationLoaded)
            {
                if (Config.IsDirty)
                {
                    Config.IsDirty = false;
                    ConfigurationChanged?.Invoke();
                    SaveAsync();
                }
            }
        }

        public InventoryToolsConfiguration Config
        {
            get;
            set;
        } = null!;

        public string ConfigurationFile
        {
            get
            {
                return _pluginInterfaceService.ConfigFile.ToString();
            }
        }
        public string InventoryFile
        {
            get
            {
                return Path.Join(_pluginInterfaceService.ConfigDirectory.FullName, "inventories.json");
            }
        }
        public string InventoryCsv
        {
            get
            {
                return Path.Join(_pluginInterfaceService.ConfigDirectory.FullName, "inventories.csv");
            }
        }
        public string MobSpawnFile
        {
            get
            {
                return Path.Join(_pluginInterfaceService.ConfigDirectory.FullName, "mob_spawns.csv");
            }
        }
        public string HistoryCsv
        {
            get
            {
                return Path.Join(_pluginInterfaceService.ConfigDirectory.FullName, "history.csv");
            }
        }

        public InventoryToolsConfiguration GetConfig()
        {
            return Config;
        }


        /// <summary>
        /// Loads the primary configuration file
        /// </summary>
        /// <param name="file">An alternate file to load</param>
        public void Load(string? file = null)
        {
            Logger.LogTrace("正在加载配置");
            Stopwatch loadConfigStopwatch = new Stopwatch();
            loadConfigStopwatch.Start();
            if (!File.Exists(file ?? ConfigurationFile))
            {
                Config = new InventoryToolsConfiguration();
                Config.MarkReloaded();
                _configurationLoaded = true;
                return;
            }
            string jsonText = File.ReadAllText(file ?? ConfigurationFile);
            var inventoryToolsConfiguration = JsonConvert.DeserializeObject<InventoryToolsConfiguration>(jsonText, new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>()
                {
                  new ColumnConverter()
                },
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                TypeNameHandling = TypeNameHandling.None,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                ContractResolver = _minifyResolver
            });
            if (inventoryToolsConfiguration == null)
            {
                Config = new InventoryToolsConfiguration();
                Config.MarkReloaded();
                _configurationLoaded = true;
                return;
            }
            loadConfigStopwatch.Stop();
            Logger.LogTrace("用时 " + loadConfigStopwatch.Elapsed.TotalSeconds + " 以加载主配置文件。");
            Config = inventoryToolsConfiguration;
            Config.MarkReloaded();
            _configurationLoaded = true;
        }

        public List<InventoryItem> LoadInventory(string? file = null)
        {
            Logger.LogTrace("正在加载背包");
            Stopwatch loadConfigStopwatch = new Stopwatch();
            loadConfigStopwatch.Start();

            var inventories = new List<InventoryItem>();


            if (!Config.InventoriesMigrated)
            {
                if (File.Exists(ConfigurationFile))
                {
                    string jsonText = File.ReadAllText(ConfigurationFile);
                    var inventoryToolsConfiguration = JsonConvert.DeserializeObject<InventoryToolsConfiguration>(
                        jsonText, new JsonSerializerSettings()
                        {
                            Converters = new List<JsonConverter>()
                            {
                                new ColumnConverter()
                            },
                            DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                            ContractResolver = _minifyResolver
                        });
                    if (inventoryToolsConfiguration != null)
                    {
                        Logger.LogTrace("正在迁移背包");
                        var temp = JObject.Parse(jsonText);
                        if (temp.ContainsKey("SavedInventories"))
                        {
                            var savedInventories = temp["SavedInventories"]
                                ?.ToObject<Dictionary<ulong, Dictionary<InventoryCategory, List<InventoryItem>>>>();

                            var parsedInventories = savedInventories ??
                                                    new Dictionary<ulong, Dictionary<
                                                        InventoryCategory,
                                                        List<InventoryItem>>>();
                            foreach (var parsedInventory in parsedInventories)
                            {
                                foreach (var category in parsedInventory.Value)
                                {
                                    foreach (var item in category.Value)
                                    {
                                        inventories.Add(item);
                                    }
                                }
                            }
                        }
                    }
                }

                Config.InventoriesMigrated = true;
            }
            else if (!Config.InventoriesMigratedToCsv)
            {
                Logger.LogTrace("已将背包标记为从 CSV 加载");
                if (File.Exists(InventoryFile))
                {
                    var parsedInventories = LoadInventoriesJson(InventoryFile) ?? new();
                    foreach (var parsedInventory in parsedInventories)
                    {
                        foreach (var category in parsedInventory.Value)
                        {
                            foreach (var item in category.Value)
                            {
                                inventories.Add(item);
                            }
                        }
                    }
                }

                Config.InventoriesMigratedToCsv = true;
            }
            else
            {
                inventories = LoadInventoriesFromCsv(out bool success);
            }
            loadConfigStopwatch.Stop();
            Logger.LogTrace("用时 " + loadConfigStopwatch.Elapsed.TotalSeconds + " 以加载背包。");
            loadConfigStopwatch.Restart();
            return inventories;
        }

        public void Save()
        {
            Stopwatch loadConfigStopwatch = new Stopwatch();
            loadConfigStopwatch.Start();
            Logger.LogTrace("正在保存亚拉戈工具箱配置");
            try
            {
                File.WriteAllText(ConfigurationFile, JsonConvert.SerializeObject(Config, Formatting.None, new JsonSerializerSettings()
                {
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                    TypeNameHandling = TypeNameHandling.None,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ContractResolver = _minifyResolver
                }));
                loadConfigStopwatch.Stop();
                Logger.LogTrace("用时 " + loadConfigStopwatch.Elapsed.TotalSeconds + " 以保存配置。");
            }
            catch (Exception e)
            {
                Logger.LogWarning($"Failed to save allagan tools configuration due to {e.Message}");
            }
        }

        private void SaveAsync()
        {
            _saveQueue.QueueBackgroundWorkItemAsync(token => Task.Run(Save, token));
        }


        /// <summary>
        /// Load a inventories json file, this is no longer the preferred format to store inventories in.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public Dictionary<ulong, Dictionary<InventoryCategory, List<InventoryItem>>>? LoadInventoriesJson(string? fileName = null)
        {
            try
            {
                fileName ??= InventoryFile;
                Logger.LogTrace("正在从以下位置加载背包：" + fileName);
                var cacheFile = new FileInfo(fileName);
                string json = File.ReadAllText(cacheFile.FullName, Encoding.UTF8);
                return JsonConvert.DeserializeObject<Dictionary<ulong, Dictionary<InventoryCategory, List<InventoryItem>>>>(json, new JsonSerializerSettings()
                {
                    DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate,
                    ContractResolver = _minifyResolver
                });
            }
            catch (Exception e)
            {
                Logger.LogError("解析已保存的背包数据时出错，" + e.Message);
                return null;
            }
        }

        private MinifyResolver _minifyResolver;
        private readonly ContainerAwareCsvLoader _containerAwareCsvLoader;
        private readonly IDalamudPluginInterface _pluginInterfaceService;
        private readonly BackgroundTaskQueue _saveQueue;

        [Obsolete]
        public void SaveInventoriesToJson(
            Dictionary<ulong, Dictionary<InventoryCategory, List<InventoryItem>>> savedInventories)
        {
            Logger.LogTrace("正在保存背包数据");
            Stopwatch loadConfigStopwatch = new Stopwatch();
            loadConfigStopwatch.Start();
            try
            {
                var items = savedInventories.SelectMany(c => c.Value.SelectMany(d => d.Value)).ToList();
                if (!SaveInventories(items))
                {
                    Logger.LogError($"Failed to save inventories due to a parsing error.");
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to save inventories due to {e.Message}");
            }
            loadConfigStopwatch.Stop();
            Logger.LogTrace("Took " + loadConfigStopwatch.Elapsed.TotalSeconds + " to save inventories.");

        }

        public Task SaveInventoriesAsync(List<InventoryItem> items)
        {
            if (_currentInventorySaveTask == null || _currentInventorySaveTask.IsCompleted || _currentInventorySaveTask.IsFaulted)
            {
                _currentInventorySaveTask = Task.Run(() => SaveInventories(items));
            }
            return _currentInventorySaveTask;
        }

        public bool SaveInventories(List<InventoryItem> items)
        {
            return _containerAwareCsvLoader.ToCsvRaw(items, Path.Join(_pluginInterfaceService.ConfigDirectory.FullName, "inventories.csv"));
        }

        public List<InventoryItem> LoadInventoriesFromCsv(out bool success, string? csvPath = null)
        {
            var inventoryCsv = csvPath ?? InventoryCsv;
            if (Path.Exists(inventoryCsv))
            {
                try
                {
                    var items = _containerAwareCsvLoader.LoadCsv<InventoryItem>(inventoryCsv,  out _, out _);
                    success = true;
                    return items;
                }
                catch (Exception e)
                {
                    success = false;
                    Logger.LogError("从 CSV 加载背包失败");
                    Logger.LogError(e.Message);
                }
            }
            else
            {
                success = true;
                Logger.LogTrace("未加载背包，文件不存在。");
            }


            return new List<InventoryItem>();
        }

        public List<InventoryChange> LoadHistoryFromCsv(out bool success, string? csvPath = null)
        {
            var historyCsv = csvPath ?? HistoryCsv;
            if (Path.Exists(historyCsv))
            {
                try
                {
                    var items = _containerAwareCsvLoader.LoadCsv<InventoryChange>(historyCsv, out _, out _);
                    success = true;
                    return items;
                }
                catch (Exception e)
                {
                    success = false;
                    Logger.LogError(e, "从 CSV 加载历史记录失败");
                }
            }
            else
            {
                success = true;
                Logger.LogTrace("未加载历史记录，文件不存在。");
            }


            return new List<InventoryChange>();
        }

        public bool SaveHistory(List<InventoryChange> changes)
        {
            return _containerAwareCsvLoader.ToCsvRaw(changes, Path.Join(_pluginInterfaceService.ConfigDirectory.FullName, "history.csv"));
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem =
                    await _saveQueue.DequeueAsync(stoppingToken);

                try
                {
                    await workItem(stoppingToken);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex,
                        "执行{WorkItem}时出错。", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Logger.LogTrace("正在停止服务{Type}（{This}）", GetType().Name, this);
            Save();
            await base.StopAsync(stoppingToken);
            Logger.LogTrace("服务{Type}（{This}）已停止", GetType().Name, this);
        }

        private bool disposed = false;
        public override void Dispose()
        {
            _framework.Update -= OnUpdate;
            disposed = true;
        }
    }
}