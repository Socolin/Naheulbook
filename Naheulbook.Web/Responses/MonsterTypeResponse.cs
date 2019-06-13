using System.Collections.Generic;

namespace Naheulbook.Web.Responses
{
    public class MonsterTypeResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<MonsterCategoryResponse> Categories { get; set; }
    }
}