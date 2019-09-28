// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Requests.Requests
{
    public class PatchGroupRequest
    {
        public string? Name { get; set; }
        public int? Mankdebol { get; set; }
        public int? Debilibeuk { get; set; }
        public NhbkDateRequest? Date { get; set; }
        public int? FighterIndex { get; set; }
    }

    public class NhbkDateRequest
    {
        public int Year { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}