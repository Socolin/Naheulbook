using System;
using Naheulbook.Core.Models;
using Naheulbook.Data.Models;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface IGroupHistoryUtil
    {
        GroupHistoryEntry CreateLogStartCombat(Group group);
        GroupHistoryEntry CreateLogEndCombat(Group group);
        GroupHistoryEntry CreateLogChangeMankdebol(Group group, int? oldValue, int newValue);
        GroupHistoryEntry CreateLogChangeDebilibeuk(Group group, int? oldValue, int newValue);
        GroupHistoryEntry CreateLogChangeDate(Group group, NhbkDate oldValue, NhbkDate newValue);
        GroupHistoryEntry CreateAddTimeLog(Group group, NhbkDate date, NhbkDateOffset timeOffset);
    }

    public class GroupHistoryUtil : IGroupHistoryUtil
    {
        private const string StartCombatActionName = "START_COMBAT";
        private const string EndCombatActionName = "END_COMBAT";
        private const string UpdateMankdebolActionName = "MANKDEBOL";
        private const string UpdateDebilibeukActionName = "DEBILIBEUK";
        private const string UpdateDateActionName = "CHANGE_DATE";
        private const string AddTimeActionName = "ADD_TIME";

        private readonly IJsonUtil _jsonUtil;

        public GroupHistoryUtil(IJsonUtil jsonUtil)
        {
            _jsonUtil = jsonUtil;
        }

        public GroupHistoryEntry CreateLogStartCombat(Group group)
        {
            return new GroupHistoryEntry
            {
                Group = group,
                Action = StartCombatActionName,
                Date = DateTime.Now,
                Gm = false
            };
        }

        public GroupHistoryEntry CreateLogEndCombat(Group group)
        {
            return new GroupHistoryEntry
            {
                Group = group,
                Action = EndCombatActionName,
                Date = DateTime.Now,
                Gm = false
            };
        }

        public GroupHistoryEntry CreateLogChangeMankdebol(Group group, int? oldValue, int newValue)
        {
            return new GroupHistoryEntry
            {
                Group = group,
                Action = UpdateMankdebolActionName,
                Date = DateTime.Now,
                Gm = true,
                Data = _jsonUtil.Serialize(new {oldValue, newValue})
            };
        }

        public GroupHistoryEntry CreateLogChangeDebilibeuk(Group group, int? oldValue, int newValue)
        {
            return new GroupHistoryEntry
            {
                Group = group,
                Action = UpdateDebilibeukActionName,
                Date = DateTime.Now,
                Gm = true,
                Data = _jsonUtil.Serialize(new {oldValue, newValue})
            };
        }

        public GroupHistoryEntry CreateLogChangeDate(Group group, NhbkDate oldValue, NhbkDate newValue)
        {
            return new GroupHistoryEntry
            {
                Group = group,
                Action = UpdateDateActionName,
                Date = DateTime.Now,
                Gm = false,
                Data = _jsonUtil.Serialize(new {oldValue, newValue})
            };
        }

        public GroupHistoryEntry CreateAddTimeLog(Group group, NhbkDate date, NhbkDateOffset timeOffset)
        {
            return new GroupHistoryEntry
            {
                Group = group,
                Action = AddTimeActionName,
                Date = DateTime.Now,
                Gm = false,
                Data = _jsonUtil.Serialize(new {timeOffset, date})
            };
        }
    }
}