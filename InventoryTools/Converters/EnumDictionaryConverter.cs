using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InventoryTools.Converters;

public class EnumDictionaryConverter : JsonConverter<Dictionary<string, Enum>>
{
    public override void WriteJson(JsonWriter writer, Dictionary<string, Enum> value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        foreach (var kvp in value)
        {
            writer.WritePropertyName(kvp.Key);
            writer.WriteStartObject();
            writer.WritePropertyName("Type");
            writer.WriteValue(kvp.Value.GetType().FullName);
            writer.WritePropertyName("Assembly");
            writer.WriteValue(kvp.Value.GetType().Assembly.GetName().Name);
            writer.WritePropertyName("Value");
            writer.WriteValue(kvp.Value.ToString());
            writer.WriteEndObject();
        }
        writer.WriteEndObject();
    }

    public override Dictionary<string, Enum> ReadJson(JsonReader reader, Type objectType, Dictionary<string, Enum> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var result = new Dictionary<string, Enum>();
        var jsonObject = JObject.Load(reader);

        foreach (var property in jsonObject.Properties())
        {
            try
            {
                string key = property.Name;
                var typeString = property.Value["Type"]?.ToString();
                var assemblyString = property.Value["Assembly"]?.ToString();
                var valueString = property.Value["Value"]?.ToString();
                if (string.IsNullOrEmpty(typeString) || string.IsNullOrEmpty(valueString))
                {
                    continue;
                }

                // 旧配置可能记录了改名前的程序集（InventoryTools），找不到时
                // 回退到本插件程序集或全局按类型名搜索，避免整个配置加载失败。
                var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(c =>
                    string.Equals(c.GetName().Name, assemblyString, StringComparison.OrdinalIgnoreCase));
                Type? enumType = assembly?.GetType(typeString);
                enumType ??= typeof(InventoryToolsPlugin).Assembly.GetType(typeString);
                if (enumType == null)
                {
                    foreach (var candidate in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        enumType = candidate.GetType(typeString);
                        if (enumType != null)
                        {
                            break;
                        }
                    }
                }

                if (enumType == null || !enumType.IsEnum)
                {
                    continue;
                }

                if (Enum.TryParse(enumType, valueString, out var enumValue) && enumValue != null)
                {
                    result[key] = (Enum)enumValue;
                }
            }
            catch
            {
                // 单个条目解析失败不应阻止整个配置加载。
            }
        }

        return result;
    }
}