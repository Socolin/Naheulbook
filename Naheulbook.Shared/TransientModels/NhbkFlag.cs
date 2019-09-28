using Newtonsoft.Json.Linq;

namespace Naheulbook.Shared.TransientModels
{
    public class NhbkFlag
    {
        public string Type { get; set; } = null!;
        public JToken? Data { get; set; }
    }
}