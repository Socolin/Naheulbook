// ReSharper disable UnusedAutoPropertyAccessor.Global

using System;

namespace Naheulbook.Web.Responses
{
    public class UserAccessTokenResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public DateTimeOffset DateCreated { get; set; }
    }

    public class UserAccessTokenResponseWithKey : UserAccessTokenResponse
    {
        public string Key { get; set; } = null!;
    }
}