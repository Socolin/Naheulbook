using System.Collections.Generic;
using Naheulbook.Data.Models;

namespace Naheulbook.Web.Responses
{
    public class ItemTemplateSectionResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Note { get; set; }
        public List<string> Specials { get; set; }
        public List<ItemTemplateCategoryResponse> Categories { get; set; }
    }
}