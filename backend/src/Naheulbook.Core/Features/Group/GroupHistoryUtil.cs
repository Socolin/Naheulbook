using System;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Group;

public interface IGroupHistoryUtil
{
    GroupHistoryEntryEntity CreateLogStartCombat(GroupEntity group);
    GroupHistoryEntryEntity CreateLogEndCombat(GroupEntity group);
    GroupHistoryEntryEntity CreateLogChangeMankdebol(GroupEntity group, int? oldValue, int newValue);
    GroupHistoryEntryEntity CreateLogChangeDebilibeuk(GroupEntity group, int? oldValue, int newValue);
    GroupHistoryEntryEntity CreateLogChangeDate(GroupEntity group, NhbkDate? oldValue, NhbkDate newValue);
    GroupHistoryEntryEntity CreateLogAddTime(GroupEntity group, NhbkDate date, NhbkDateOffset timeOffset);
    GroupHistoryEntryEntity CreateLogEventRp(GroupEntity @group, bool isGm, string info);
}

public class GroupHistoryUtil(IJsonUtil jsonUtil) : IGroupHistoryUtil
{
    private const string StartCombatActionName = "START_COMBAT";
    private const string EndCombatActionName = "END_COMBAT";
    private const string UpdateMankdebolActionName = "MANKDEBOL";
    private const string UpdateDebilibeukActionName = "DEBILIBEUK";
    private const string UpdateDateActionName = "CHANGE_DATE";
    private const string AddTimeActionName = "ADD_TIME";
    private const string EventRpActionName = "EVENT_RP";

    public GroupHistoryEntryEntity CreateLogStartCombat(GroupEntity group)
    {
        return new GroupHistoryEntryEntity
        {
            Group = group,
            Action = StartCombatActionName,
            Date = DateTime.Now,
            Gm = false,
        };
    }

    public GroupHistoryEntryEntity CreateLogEndCombat(GroupEntity group)
    {
        return new GroupHistoryEntryEntity
        {
            Group = group,
            Action = EndCombatActionName,
            Date = DateTime.Now,
            Gm = false,
        };
    }

    public GroupHistoryEntryEntity CreateLogChangeMankdebol(GroupEntity group, int? oldValue, int newValue)
    {
        return new GroupHistoryEntryEntity
        {
            Group = group,
            Action = UpdateMankdebolActionName,
            Date = DateTime.Now,
            Gm = true,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public GroupHistoryEntryEntity CreateLogChangeDebilibeuk(GroupEntity group, int? oldValue, int newValue)
    {
        return new GroupHistoryEntryEntity
        {
            Group = group,
            Action = UpdateDebilibeukActionName,
            Date = DateTime.Now,
            Gm = true,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public GroupHistoryEntryEntity CreateLogChangeDate(GroupEntity group, NhbkDate? oldValue, NhbkDate newValue)
    {
        return new GroupHistoryEntryEntity
        {
            Group = group,
            Action = UpdateDateActionName,
            Date = DateTime.Now,
            Gm = false,
            Data = jsonUtil.Serialize(new {oldValue, newValue}),
        };
    }

    public GroupHistoryEntryEntity CreateLogAddTime(GroupEntity group, NhbkDate date, NhbkDateOffset timeOffset)
    {
        return new GroupHistoryEntryEntity
        {
            Group = group,
            Action = AddTimeActionName,
            Date = DateTime.Now,
            Gm = false,
            Data = jsonUtil.Serialize(new {timeOffset, date}),
        };
    }

    public GroupHistoryEntryEntity CreateLogEventRp(GroupEntity @group, bool isGm, string info)
    {
        return new GroupHistoryEntryEntity
        {
            Group = group,
            Action = EventRpActionName,
            Date = DateTime.Now,
            Gm = isGm,
            Info = info,
        };
    }
}