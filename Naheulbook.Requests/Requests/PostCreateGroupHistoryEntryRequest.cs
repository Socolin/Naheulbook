namespace Naheulbook.Requests.Requests
{
    public class PostCreateGroupHistoryEntryRequest
    {
        public bool IsGm { get; set; }
        public string Info { get; set; } = null!;
    }
}