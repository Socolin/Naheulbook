using Newtonsoft.Json.Linq;

namespace Naheulbook.Shared.TransientModels
{
    public class PartialItemTemplateData
    {
        public int? Charge { get; set; }
        public JObject Icon { get; set; }
        public JObject Lifetime { get; set; }
        public string NotIdentifiedName { get; set; }
    }
}