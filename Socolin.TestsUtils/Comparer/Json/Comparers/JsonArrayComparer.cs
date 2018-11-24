using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Socolin.TestsUtils.Comparer.Json.Errors;
using Socolin.TestsUtils.Comparer.Json.Utils;

namespace Socolin.TestsUtils.Comparer.Json.Comparers
{
    public interface IJsonArrayComparer
    {
        IEnumerable<IJsonCompareError<JToken>> Compare(JArray expected, JArray actual, IJsonComparer jsonComparer, string path = "");
    }

    public class JsonArrayComparer : IJsonArrayComparer
    {
        public IEnumerable<IJsonCompareError<JToken>> Compare(JArray expected, JArray actual, IJsonComparer jsonComparer, string path = "")
        {
            if (expected.Count != actual.Count)
            {
                yield return new InvalidSizeJsonCompareError(path, expected, actual);
                yield break;
            }

            for (var i = 0; i < expected.Count; i++)
            {
                var expectedElement = expected[i];
                var actualElement = actual[i];

                var errors = jsonComparer.Compare(expectedElement, actualElement, JsonPathUtils.Combine(path, $"[{i}]"));

                foreach (var error in errors)
                {
                    yield return error;
                }
            }
        }
    }
}