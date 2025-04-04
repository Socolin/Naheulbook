using System;

namespace Naheulbook.Core.Features.Users;

public class UserNotFoundException(int? userId = null) : Exception
{
    public int? UserId { get; } = userId;
}