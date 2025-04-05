using Naheulbook.Data.EntityFrameworkCore.Entities;

namespace Naheulbook.TestUtils;

public partial class TestDataUtil
{
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
        return AddGroupInvite(out _, fromGroup, customizer);
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
        return AddEvent(out _, customizer);
    }

    public TestDataUtil AddEvent(out EventEntity @event, Action<EventEntity> customizer = null)
    {
        var group = GetLast<GroupEntity>();

        @event = new EventEntity
        {
            Name = RngUtil.GetRandomString("some-event-name"),
            Description = RngUtil.GetRandomString("some-event-description"),
            GroupId = group.Id,
            Timestamp = 721487,
        };

        return SaveEntity(@event, customizer);
    }

    public TestDataUtil AddGroupHistoryEntry(Action<GroupHistoryEntryEntity> customizer = null)
    {
        return AddGroupHistoryEntry(out _, customizer);
    }

    public TestDataUtil AddGroupHistoryEntry(out GroupHistoryEntryEntity groupHistoryEntry, Action<GroupHistoryEntryEntity> customizer = null)
    {
        var group = GetLast<GroupEntity>();

        groupHistoryEntry = new GroupHistoryEntryEntity
        {
            Data = "{}",
            Gm = true,
            Date = new DateTime(2020, 10, 5, 5, 7, 8, DateTimeKind.Utc),
            GroupId = group.Id,
            Action = RngUtil.GetRandomString("some-group-history-action"),
            Info = RngUtil.GetRandomString("some-info"),
        };

        return SaveEntity(groupHistoryEntry, customizer);
    }
}