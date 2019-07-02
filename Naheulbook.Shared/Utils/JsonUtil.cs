using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Naheulbook.Shared.Utils
{
    public interface IJsonUtil
    {
        string Serialize(object obj);
        T Deserialize<T>(string json) where T : class;
    }

    public class JsonUtil : IJsonUtil
    {
        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, _serializerSettings);
        }

        public T Deserialize<T>(string json) where T : class
        {
            if (json == null)
                return null;
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}