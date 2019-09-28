using System.Collections.Generic;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Naheulbook.Web.Responses
{
    public class DescribedFlagResponse
    {
        public string Description { get; set; } = null!;
        public List<FlagResponse> Flags { get; set; } = null!;
    }
}