using System;

namespace Naheulbook.Core.Exceptions;

public class UserNotFoundException : Exception
{
    public int? UserId { get; }

    public UserNotFoundException(int? userId = null)
    {
        UserId = userId;
    }
}