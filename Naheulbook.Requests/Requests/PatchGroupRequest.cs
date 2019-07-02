namespace Naheulbook.Requests.Requests
{
    public class PatchGroupRequest
    {
        public int? Mankdebol { get; set; }
        public int? Debilibeuk { get; set; }
        public NhbkDateRequest Date { get; set; }
    }

    public class NhbkDateRequest
    {
        public int Year { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}