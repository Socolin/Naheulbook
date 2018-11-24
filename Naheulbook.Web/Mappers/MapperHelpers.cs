using System;
using Newtonsoft.Json;

namespace Naheulbook.Web.Mappers
{
    public class MapperHelpers
    {
        public static string[] FromCommaSeparatedStringArray(string str)
        {
            return str.Split(',', StringSplitOptions.RemoveEmptyEntries);
        }

        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}