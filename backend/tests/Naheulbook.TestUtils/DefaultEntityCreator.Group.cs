using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

[Obsolete]
public partial class DefaultEntityCreator
{
    public GroupEntity CreateGroup(int masterId, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new GroupEntity
        {
            Name = $"some-name-{suffix}",
            Data = "{}",
            MasterId = masterId,
        };
    }

    public EventEntity CreateEvent(GroupEntity group, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new EventEntity
        {
            Name = $"some-event-name-{suffix}",
            Description = $"some-event-description-{suffix}",
            GroupId = group.Id,
            Timestamp = 721487,
        };
    }

    public GroupHistoryEntryEntity CreateGroupHistory(GroupEntity group, string suffix = null)
    {
        if (suffix == null)
            suffix = RngUtil.GetRandomHexString(8);

        return new GroupHistoryEntryEntity
        {
            Data = "{}",
            Gm = true,
            Date = new DateTime(2020, 10, 5, 5, 7, 8, DateTimeKind.Utc),
            GroupId = group.Id,
            Action = $"some-group-history-action-{suffix}",
            Info = $"some-info-{suffix}",
        };
    }
}