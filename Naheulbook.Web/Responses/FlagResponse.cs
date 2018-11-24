using Newtonsoft.Json.Linq;

namespace Naheulbook.Web.Responses
{
    public class FlagResponse
    {
        public string Type { get; set; }
        public JToken Data { get; set; }
    }
}