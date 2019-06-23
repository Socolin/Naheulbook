using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddGroup(int masterId, Action<Group> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateGroup(masterId, GetLast<Location>()), customizer);
        }

        public TestDataUtil AddGroup(Action<Group> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateGroup(GetLast<User>().Id, GetLast<Location>()), customizer);
        }

        public TestDataUtil AddLoot(Action<Loot> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateLoot(GetLast<Group>()), customizer);
        }

        public TestDataUtil AddGroupInvite(Character character, Group group, bool fromGroup)
        {
            var groupInvite = new GroupInvite
            {
                FromGroup = fromGroup,
                Character = character,
                CharacterId = character.Id,
                Group = group,
                GroupId = group.Id
            };
            return SaveEntity(groupInvite, null);
        }

        public TestDataUtil AddEvent(Action<Event> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateEvent(GetLast<Group>()), customizer);
        }

        public TestDataUtil AddGroupHistory(Action<GroupHistory> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateGroupHistory(GetLast<Group>()), customizer);
        }
    }
}