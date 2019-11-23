using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class DefaultEntityCreator
    {
        public Npc CreateNpc(Group group, string suffix = null)
        {
            if (suffix == null)
                suffix = RngUtil.GetRandomHexString(8);

            return new Npc()
            {
                Data = "{}",
                GroupId = group.Id,
                Name = $"some-npc-name-{suffix}"
            };
        }
    }
}