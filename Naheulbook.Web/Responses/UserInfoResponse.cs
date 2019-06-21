namespace Naheulbook.Web.Responses
{
    public class UserInfoResponse
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public bool Admin { get; set; }
        public bool LinkedWithFb { get; set; }
        public bool LinkedWithGoogle { get; set; }
        public bool LinkedWithTwitter { get; set; }
        public bool LinkedWithLive { get; set; }
    }
}