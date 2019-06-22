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
    }
}