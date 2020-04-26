using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public Group CreateGroup(int masterId, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Group
            {
                Name = $"some-name-{suffix}",
                Data = "{}",
                MasterId = masterId
            };
        }

        public Loot CreateLoot(Group group, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Loot
            {
                Name = $"some-name-{suffix}",
                Group = group,
                GroupId = group.Id,
                IsVisibleForPlayer = false
            };
        }

        public Event CreateEvent(Group group, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Event
            {
                Name = $"some-event-name-{suffix}",
                Description = $"some-event-description-{suffix}",
                GroupId = group.Id,
                Timestamp = 721487
            };
        }

        public GroupHistoryEntry CreateGroupHistory(Group group, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new GroupHistoryEntry
            {
                Data = "{}",
                Gm = true,
                Date = new DateTime(2020, 10, 5, 5, 7, 8, DateTimeKind.Utc),
                GroupId = group.Id,
                Action = $"some-group-history-action-{suffix}"
            };
        }
    }
}