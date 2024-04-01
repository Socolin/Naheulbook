using System;

namespace Naheulbook.Core.Exceptions;

public class UserNotFoundException(int? userId = null) : Exception
{
    public int? UserId { get; } = userId;
}