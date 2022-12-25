using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils
{
    public partial class TestDataUtil
    {
        public TestDataUtil AddGroup(int masterId, Action<GroupEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateGroup(masterId), customizer);
        }

        public TestDataUtil AddGroup(Action<GroupEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateGroup(GetLast<UserEntity>().Id), customizer);
        }

        public TestDataUtil AddGroupWithRequiredData(Action<GroupEntity> customizer = null)
        {
            AddUser();
            return SaveEntity(_defaultEntityCreator.CreateGroup(GetLast<UserEntity>().Id), customizer);
        }

        public TestDataUtil AddLoot(Action<LootEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateLoot(GetLast<GroupEntity>()), customizer);
        }

        public TestDataUtil AddGroupInvite(CharacterEntity character, GroupEntity group, bool fromGroup)
        {
            var groupInvite = new GroupInviteEntity
            {
                FromGroup = fromGroup,
                Character = character,
                CharacterId = character.Id,
                Group = group,
                GroupId = group.Id
            };
            return SaveEntity(groupInvite, null);
        }

        public TestDataUtil AddEvent(Action<EventEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateEvent(GetLast<GroupEntity>()), customizer);
        }

        public TestDataUtil AddGroupHistoryEntry(Action<GroupHistoryEntryEntity> customizer = null)
        {
            return SaveEntity(_defaultEntityCreator.CreateGroupHistory(GetLast<GroupEntity>()), customizer);
        }
    }
}