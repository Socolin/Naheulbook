using Microsoft.AspNetCore.Http;

namespace Naheulbook.Web.Extensions
{
    public static class SessionExtension
    {
        private const string UserIdKey = "userId";

        public static int? GetCurrentUserId(this ISession session)
        {
            return session.GetInt32(UserIdKey);
        }

        public static void SetCurrentUserId(this ISession session, int? userId)
        {
            if (userId == null)
                session.Remove(UserIdKey);
            else
                session.SetInt32(UserIdKey, userId.Value);
        }
    }
}