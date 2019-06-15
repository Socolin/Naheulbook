using System.Collections.Generic;

namespace Naheulbook.Shared.TransientModels
{
    public class StatModifier
    {
        public string Stat { get; set; }
        public string Type { get; set; }
        public short Value { get; set; }
        public IList<string> Special { get; set; }
    }
}