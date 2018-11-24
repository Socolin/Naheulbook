using Newtonsoft.Json.Linq;
using Socolin.TestsUtils.Comparer.Json;
using Socolin.TestsUtils.Comparer.Json.Errors;

namespace Socolin.TestUtils.Tests.Unit.Comparer.Json.Errors
{
    internal class TestJsonCompareError : JsonCompareError
    {
        public TestJsonCompareError(string path = null, JToken actualValue = null, JToken expectedValue = null)
            : base(path, expectedValue, actualValue)
        {
        }

        public override string Message => "some-message";
    }
}