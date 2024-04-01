using System.Collections.Generic;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils;

public interface IGroupUtil
{
    void ApplyChangesAndNotify(GroupEntity group, PatchGroupRequest request, INotificationSession notificationSession);
    void StartCombat(GroupEntity group, INotificationSession notificationSession);
    void EndCombat(GroupEntity group, INotificationSession notificationSession);
    NhbkDate AddTimeAndNotify(GroupEntity @group, NhbkDateOffset request, INotificationSession notificationSession);
}

public class GroupUtil(
    IJsonUtil jsonUtil,
    IGroupHistoryUtil groupHistoryUtil
) : IGroupUtil
{
    public void ApplyChangesAndNotify(GroupEntity group, PatchGroupRequest request, INotificationSession notificationSession)
    {
        var groupData = jsonUtil.Deserialize<GroupData>(group.Data) ?? new GroupData();

        if (request.Mankdebol.HasValue)
        {
            group.AddHistoryEntry(groupHistoryUtil.CreateLogChangeMankdebol(group, groupData.Mankdebol, request.Mankdebol.Value));
            groupData.Mankdebol = request.Mankdebol.Value;
        }

        if (request.Debilibeuk.HasValue)
        {
            group.AddHistoryEntry(groupHistoryUtil.CreateLogChangeDebilibeuk(group, groupData.Debilibeuk, request.Debilibeuk.Value));
            groupData.Debilibeuk = request.Debilibeuk.Value;
        }

        if (request.Date != null)
        {
            var newDate = new NhbkDate(request.Date);
            group.AddHistoryEntry(groupHistoryUtil.CreateLogChangeDate(group, groupData.Date, newDate));
            groupData.Date = newDate;
        }

        if (request.FighterIndex != null)
        {
            groupData.CurrentFighterIndex = request.FighterIndex.Value;
        }

        if (request.Name != null)
        {
            group.Name = request.Name;
        }

        notificationSession.NotifyGroupChangeGroupData(group.Id, groupData);

        group.Data = jsonUtil.Serialize(groupData);
    }

    public void StartCombat(GroupEntity group, INotificationSession notificationSession)
    {
        var groupData = jsonUtil.Deserialize<GroupData>(group.Data) ?? new GroupData();
        if (groupData.InCombat == true)
            throw new GroupAlreadyInCombatException();

        groupData.InCombat = true;

        var loot = new LootEntity
        {
            Name = "Combat",
            GroupId = group.Id,
            Items = new List<ItemEntity>(),
            Monsters = new List<MonsterEntity>(),
        };
        group.CombatLoot = loot;
        group.CombatLootId = loot.Id;

        group.AddHistoryEntry(groupHistoryUtil.CreateLogStartCombat(group));

        notificationSession.NotifyGroupChangeGroupData(group.Id, groupData);

        group.Data = jsonUtil.Serialize(groupData);
    }

    public void EndCombat(GroupEntity group, INotificationSession notificationSession)
    {
        var groupData = jsonUtil.Deserialize<GroupData>(group.Data) ?? new GroupData();
        if (groupData.InCombat != true)
            throw new GroupNotInCombatException();

        groupData.InCombat = false;

        group.AddHistoryEntry(groupHistoryUtil.CreateLogEndCombat(group));

        notificationSession.NotifyGroupChangeGroupData(group.Id, groupData);

        group.Data = jsonUtil.Serialize(groupData);
    }

    public NhbkDate AddTimeAndNotify(GroupEntity group, NhbkDateOffset timeOffset, INotificationSession notificationSession)
    {
        var groupData = jsonUtil.Deserialize<GroupData>(group.Data) ?? new GroupData();
        if (groupData.Date == null)
            throw new GroupDateNotSetException();

        groupData.Date.Add(timeOffset);

        group.AddHistoryEntry(groupHistoryUtil.CreateLogAddTime(group, groupData.Date, timeOffset));
        notificationSession.NotifyGroupChangeGroupData(group.Id, groupData);

        group.Data = jsonUtil.Serialize(groupData);

        return groupData.Date;
    }
}