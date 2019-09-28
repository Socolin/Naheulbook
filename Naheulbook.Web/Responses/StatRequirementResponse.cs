// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Web.Responses
{
    public class StatRequirementResponse
    {
        public string Stat { get; set; } = null!;
        public int? Min { get; set; }
        public int? Max { get; set; }
    }
}