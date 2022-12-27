using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddGroup(int masterId, Action<GroupEntity> customizer = null)
    {
        return SaveEntity(_defaultEntityCreator.CreateGroup(masterId), customizer);
    }

    public TestDataUtil AddGroup(Action<GroupEntity> customizer = null)
    {
        return AddGroup(out _, customizer);
    }

    public TestDataUtil AddGroup(out GroupEntity group, Action<GroupEntity> customizer = null)
    {
        var userEntity = GetLast<UserEntity>();
        group = _defaultEntityCreator.CreateGroup(userEntity.Id);
        return SaveEntity(group, customizer);
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

    public TestDataUtil AddGroupInvite(bool fromGroup, Action<GroupInviteEntity> customizer = null)
    {
        return AddGroupInvite(out var _, fromGroup, customizer);
    }

    public TestDataUtil AddGroupInvite(out GroupInviteEntity groupInvite, bool fromGroup, Action<GroupInviteEntity> customizer = null)
    {
        groupInvite = new GroupInviteEntity
        {
            FromGroup = fromGroup,
            CharacterId = GetLast<CharacterEntity>().Id,
            GroupId = GetLast<GroupEntity>().Id,
        };
        return SaveEntity(groupInvite, customizer);
    }

    public TestDataUtil AddEvent(Action<EventEntity> customizer = null)
    {
        return SaveEntity(_defaultEntityCreator.CreateEvent(GetLast<GroupEntity>()), customizer);
    }

    public TestDataUtil AddGroupHistoryEntry(Action<GroupHistoryEntryEntity> customizer = null)
    {
        return AddGroupHistoryEntry(out _, customizer);
    }

    public TestDataUtil AddGroupHistoryEntry(out GroupHistoryEntryEntity groupHistoryEntry, Action<GroupHistoryEntryEntity> customizer = null)
    {
        var group = GetLast<GroupEntity>();
        groupHistoryEntry = _defaultEntityCreator.CreateGroupHistory(group);
        return SaveEntity(groupHistoryEntry, customizer);
    }
}