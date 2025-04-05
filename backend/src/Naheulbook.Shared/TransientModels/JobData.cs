using Newtonsoft.Json.Linq;

namespace Naheulbook.Shared.TransientModels;

public class JobData
{
    public JObject ForOrigin { get; set; } = null!;
}