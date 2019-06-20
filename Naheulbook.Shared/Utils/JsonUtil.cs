using Newtonsoft.Json;

namespace Naheulbook.Shared.Utils
{
    public interface IJsonUtil
    {
        string Serialize(object obj);
        T Deserialize<T>(string json);
    }

    public class JsonUtil : IJsonUtil
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}