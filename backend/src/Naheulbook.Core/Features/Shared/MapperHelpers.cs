using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json;

namespace Naheulbook.Core.Features.Shared;

public static class MapperHelpers
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        NullValueHandling = NullValueHandling.Ignore,
        Formatting = Formatting.None,
    };

    public static string[] FromCommaSeparatedStringArray(string? str)
    {
        return str?.Split(',', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();
    }

    public static T? FromJson<T>(string? json)
        where T : class
    {
        if (string.IsNullOrEmpty(json))
            return null;

        return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
    }

    public static T FromJsonNotNull<T>(string? json)
        where T : class, new()
    {
        if (string.IsNullOrEmpty(json))
            return new T();

        return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings) ?? new T();
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
            return null;

        return list.Split(",").ToList();
    }

    /// <summary>
    /// Format a date stored as UTC (but read as <see cref="DateTimeKind.Unspecified"/> from the database)
    /// as an ISO 8601 string with the `Z` suffix, so the clients can convert it to their local time.
    /// </summary>
    public static string ToUtcIsoString(DateTime date)
    {
        return DateTime.SpecifyKind(date, DateTimeKind.Utc).ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", System.Globalization.CultureInfo.InvariantCulture);
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