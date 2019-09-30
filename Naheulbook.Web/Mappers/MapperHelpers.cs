using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Naheulbook.Web.Mappers
{
    public class MapperHelpers
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };

        public static string[] FromCommaSeparatedStringArray(string? str)
        {
            return str?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? new string[0];
        }

        public static T? FromJson<T>(string? json)
            where T : class
        {
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }

        public static T FromJsonNotNull<T>(string? json)
            where T : class, new()
        {
            if (string.IsNullOrEmpty(json))
                return new T();

            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }

        public static string? ToJson(object? obj)
        {
            if (obj == null)
                return null;

            return JsonConvert.SerializeObject(obj);
        }

        public static List<string>? FromCommaSeparatedList(string? list)
        {
            if (string.IsNullOrEmpty(list))
                return default;

            return list!.Split(",").ToList();
        }

        public static string? FromDateTimeToString(DateTime? date)
        {
            return date?.ToString("s");
        }

        public static string? FromDateTimeToString(DateTimeOffset? date)
        {
            return date?.ToString("s");
        }
    }
}