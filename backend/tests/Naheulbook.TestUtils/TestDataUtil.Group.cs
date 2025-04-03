using System;
using Naheulbook.Data.Models;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
    public TestDataUtil AddGroup(int masterId, Action<GroupEntity> customizer = null)
    {
        return SaveEntity(defaultEntityCreator.CreateGroup(masterId), customizer);
    }

    public TestDataUtil AddGroup(Action<GroupEntity> customizer = null)
    {
        return AddGroup(out _, customizer);
    }

    public TestDataUtil AddGroup(out GroupEntity group, Action<GroupEntity> customizer = null)
    {
        var user = GetLast<UserEntity>();

        group = new GroupEntity
        {
            Name = RngUtil.GetRandomString("some-group-name"),
            Data = "{}",
            MasterId = user.Id,
        };

        return SaveEntity(group, customizer);
    }

    public TestDataUtil AddLoot(Action<LootEntity> customizer = null)
    {
        return AddLoot(out _, customizer);
    }

    public TestDataUtil AddLoot(out LootEntity loot, Action<LootEntity> customizer = null)
    {
        var group = GetLast<GroupEntity>();

        loot = new LootEntity
        {
            Name = RngUtil.GetRandomString("some-loot-name"),
            GroupId = group.Id,
            IsVisibleForPlayer = false,
        };

        return SaveEntity(loot, customizer);
    }

    public TestDataUtil AddGroupInvite(CharacterEntity character, GroupEntity group, bool fromGroup)
    {
        var groupInvite = new GroupInviteEntity
        {
            FromGroup = fromGroup,
            Character = character,
            CharacterId = character.Id,
            Group = group,
            GroupId = group.Id,
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
        return SaveEntity(defaultEntityCreator.CreateEvent(GetLast<GroupEntity>()), customizer);
    }

    public TestDataUtil AddGroupHistoryEntry(Action<GroupHistoryEntryEntity> customizer = null)
    {
        return AddGroupHistoryEntry(out _, customizer);
    }

    public TestDataUtil AddGroupHistoryEntry(out GroupHistoryEntryEntity groupHistoryEntry, Action<GroupHistoryEntryEntity> customizer = null)
    {
        var group = GetLast<GroupEntity>();
        groupHistoryEntry = defaultEntityCreator.CreateGroupHistory(group);
        return SaveEntity(groupHistoryEntry, customizer);
    }
}