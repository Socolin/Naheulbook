using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Socolin.TestsUtils.Comparer.Json.Comparers;
using Socolin.TestsUtils.Comparer.Json.Errors;

namespace Socolin.TestsUtils.Comparer.Json
{
    public interface IJsonComparer
    {
        IList<IJsonCompareError<JToken>> Compare(string expectedJson, string actualJson);
        IList<IJsonCompareError<JToken>> Compare(JToken expected, JToken actual);
        IEnumerable<IJsonCompareError<JToken>> Compare(JToken expected, JToken actual, string path);
    }

    public class JsonComparer : IJsonComparer
    {
        private static readonly JsonCompareError[] EmptyErrors = new JsonCompareError[0];
        private readonly IJsonObjectComparer _jsonObjectComparer;
        private readonly IJsonArrayComparer _jsonArrayComparer;
        private readonly IJsonValueComparer _jsonValueComparer;

        public static JsonComparer GetDefault()
        {
            return new JsonComparer(new JsonObjectComparer(), new JsonArrayComparer(), new JsonValueComparer());
        }

        public JsonComparer(IJsonObjectComparer jsonObjectComparer, IJsonArrayComparer jsonArrayComparer, IJsonValueComparer jsonValueComparer)
        {
            _jsonObjectComparer = jsonObjectComparer;
            _jsonArrayComparer = jsonArrayComparer;
            _jsonValueComparer = jsonValueComparer;
        }

        public IList<IJsonCompareError<JToken>> Compare(string expectedJson, string actualJson)
        {
            var expected = JsonConvert.DeserializeObject<JToken>(expectedJson);
            var actual = JsonConvert.DeserializeObject<JToken>(actualJson);
            return Compare(expected, actual);
        }

        public IList<IJsonCompareError<JToken>> Compare(JToken expected, JToken actual)
        {
            return Compare(expected, actual, "").ToList();
        }

        public IEnumerable<IJsonCompareError<JToken>> Compare(JToken expected, JToken actual, string path)
        {
            if (expected.Type != actual.Type)
            {
                yield return new InvalidTypeJsonCompareError(path, expected, actual);
                yield break;
            }

            IEnumerable<IJsonCompareError<JToken>> errors = EmptyErrors;
            switch (actual.Type)
            {
                case JTokenType.Object:
                    errors = _jsonObjectComparer.Compare(expected as JObject, actual as JObject, this, path);
                    break;
                case JTokenType.Array:
                    errors = _jsonArrayComparer.Compare(expected as JArray, actual as JArray, this, path);
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Null:
                case JTokenType.Undefined:
                    errors = _jsonValueComparer.Compare(expected as JValue, actual as JValue, path);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(actual.Type), actual.Type, "Cannot compare this type");
            }

            foreach (var jsonCompareError in errors)
            {
                yield return jsonCompareError;
            }
        }
    }
}