using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions;

public class GroupNotFoundException(int groupId) : Exception
{
    public int GroupId { get; } = groupId;
}