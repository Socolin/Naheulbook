namespace Naheulbook.Web.Responses
{
    public class UserJwtResponse
    {
        public string Token { get; set; }
        public UserInfoResponse UserInfo { get; set; }
    }
}