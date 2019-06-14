using System.Collections.Generic;

namespace Naheulbook.Web.Responses
{
    public class MonsterTraitResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public IList<string> Levels { get; set; }
    }
}