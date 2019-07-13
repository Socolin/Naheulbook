using System.Collections.Generic;
using Naheulbook.Shared.TransientModels;

namespace Naheulbook.Requests.Requests
{
    public class PostGroupUpdateDurationsRequest
    {
        public int? MonsterId { get; set; }
        public int? CharacterId { get; set; }
        public IList<DurationChangeRequest> Changes { get; set; }
    }

    public class DurationChangeRequest
    {
        public string Type { get; set; }
        public int? ItemId { get; set; }
        public NewModifierDurationValue Modifier { get; set; }
        public LifeTime LifeTime { get; set; }
    }
}