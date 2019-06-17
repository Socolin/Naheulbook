using Newtonsoft.Json.Linq;

namespace Naheulbook.Requests.Requests
{
    public class CreateItemRequest
    {
        public int ItemTemplateId { get; set; }
        public JObject ItemData { get; set; }
    }
}