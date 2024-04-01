using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Naheulbook.Shared.Utils;

public interface IJsonUtil
{
    string? Serialize(object? obj);
    string SerializeNonNull<T>(T obj) where T : class;
    T? Deserialize<T>(string? json) where T : class;
    T DeserializeOrCreate<T>(string? json) where T : class, new();
}

public class JsonUtil : IJsonUtil
{
    private readonly JsonSerializerSettings _serializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
    };

    public string? Serialize(object? obj)
    {
        if (obj == null)
            return null;
        return JsonConvert.SerializeObject(obj, _serializerSettings);
    }

    public string SerializeNonNull<T>(T obj) where T: class
    {
        return JsonConvert.SerializeObject(obj, _serializerSettings);
    }

    public T? Deserialize<T>(string? json)
        where T : class
    {
        if (json == null)
            return null;
        return JsonConvert.DeserializeObject<T>(json);
    }

    public T DeserializeOrCreate<T>(string? json)
        where T : class, new()
    {
        if (string.IsNullOrEmpty(json))
            return new T();
        var result = JsonConvert.DeserializeObject<T?>(json);
        return result ?? new T();
    }
}