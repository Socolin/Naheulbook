using System;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Naheulbook.Core.Exceptions
{
    public class GroupNotFoundException : Exception
    {
        public int GroupId { get; }

        public GroupNotFoundException(int groupId)
        {
            GroupId = groupId;
        }
    }
}