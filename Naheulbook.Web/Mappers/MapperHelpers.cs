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

        public static string[] FromCommaSeparatedStringArray(string str)
        {
            return str?.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }

        public static string ToJson(object obj)
        {
            if (obj == null)
                return null;

            return JsonConvert.SerializeObject(obj);
        }

        public static List<string> FromCommaSeparatedList(string list)
        {
            if (string.IsNullOrEmpty(list))
                return default;

            return list.Split(",").ToList();
        }

        public static string FromDateTimeToString(DateTime? date)
        {
            return date?.ToString("s");
        }

        public static string FromDateTimeToString(DateTimeOffset? date)
        {
            return date?.ToString("s");
        }
    }
}