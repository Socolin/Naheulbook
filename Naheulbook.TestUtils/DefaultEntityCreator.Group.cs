using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public Group CreateGroup(int masterId, Location location, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Group
            {
                Name = $"some-name-{suffix}",
                Data = "{}",
                MasterId = masterId,
                Location = location
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

        public GroupHistory CreateGroupHistory(Group group, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new GroupHistory
            {
                Data = "{}",
                Gm = true,
                Date = new DateTime(2020, 10, 5, 5, 7, 8, DateTimeKind.Utc),
                GroupId = group.Id,
                Action = $"some-action-{suffix}",
                Info = $"some-info-{suffix}",
                UserId = group.MasterId
            };
        }
    }
}