using System;
using System.Collections;
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
            return str.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        public static T FromJson<T>(string json)
        {
            if (string.IsNullOrEmpty(json))
                return default;

            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }

        public static List<string> FromCommaSeparatedList(string list)
        {
            if (string.IsNullOrEmpty(list))
                return default;

            return list.Split(",").ToList();
        }
    }
}