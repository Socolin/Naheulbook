using System.Collections.Generic;

namespace Naheulbook.Requests.Requests
{
    public class CreateItemTemplateSectionRequest
    {
        public string Name { get; set; }
        public string Note { get; set; }
        public List<string> Specials { get; set; }
    }
}