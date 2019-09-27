using System.Collections.Generic;

namespace Naheulbook.Requests.Requests
{
    public class CreateItemTemplateSectionRequest
    {
        public string Name { get; set; } = null!;
        public string Note { get; set; } = null!;
        public List<string> Specials { get; set; } = null!;
    }
}