using Newtonsoft.Json;

namespace Naheulbook.Core.Utils
{
    public interface IJsonUtil
    {
        string Serialize(object obj);
    }

    public class JsonUtil : IJsonUtil
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}