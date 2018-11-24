using System.Collections.Generic;

namespace Naheulbook.Web.Responses
{
    public class DescribedFlagResponse
    {
        public string Description { get; set; }
        public List<FlagResponse> Flags { get; set; }
    }
}