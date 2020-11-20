using System.Collections.Generic;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface IGroupUtil
    {
        void ApplyChangesAndNotify(Group group, PatchGroupRequest request, INotificationSession notificationSession);
        void StartCombat(Group group, INotificationSession notificationSession);
        void EndCombat(Group group, INotificationSession notificationSession);
        NhbkDate AddTimeAndNotify(Group @group, NhbkDateOffset request, INotificationSession notificationSession);
    }

    public class GroupUtil : IGroupUtil
    {
        private readonly IJsonUtil _jsonUtil;
        private readonly IGroupHistoryUtil _groupHistoryUtil;

        public GroupUtil(
            IJsonUtil jsonUtil,
            IGroupHistoryUtil groupHistoryUtil
        )
        {
            _jsonUtil = jsonUtil;
            _groupHistoryUtil = groupHistoryUtil;
        }

        public void ApplyChangesAndNotify(Group group, PatchGroupRequest request, INotificationSession notificationSession)
        {
            var groupData = _jsonUtil.Deserialize<GroupData>(group.Data) ?? new GroupData();

            if (request.Mankdebol.HasValue)
            {
                group.AddHistoryEntry(_groupHistoryUtil.CreateLogChangeMankdebol(group, groupData.Mankdebol, request.Mankdebol.Value));
                groupData.Mankdebol = request.Mankdebol.Value;
            }

            if (request.Debilibeuk.HasValue)
            {
                group.AddHistoryEntry(_groupHistoryUtil.CreateLogChangeDebilibeuk(group, groupData.Debilibeuk, request.Debilibeuk.Value));
                groupData.Debilibeuk = request.Debilibeuk.Value;
            }

            if (request.Date != null)
            {
                var newDate = new NhbkDate(request.Date);
                group.AddHistoryEntry(_groupHistoryUtil.CreateLogChangeDate(group, groupData.Date, newDate));
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

            group.Data = _jsonUtil.Serialize(groupData);
        }

        public void StartCombat(Group group, INotificationSession notificationSession)
        {
            var groupData = _jsonUtil.Deserialize<GroupData>(group.Data) ?? new GroupData();
            if (groupData.InCombat == true)
                throw new GroupAlreadyInCombatException();

            groupData.InCombat = true;

            group.Loots ??= new List<Loot>();
            var loot = new Loot
            {
                Name = "Combat",
                GroupId = group.Id
            };
            group.CombatLoot = loot;

            group.AddHistoryEntry(_groupHistoryUtil.CreateLogStartCombat(group));

            notificationSession.NotifyGroupChangeGroupData(group.Id, groupData);

            group.Data = _jsonUtil.Serialize(groupData);
        }

        public void EndCombat(Group group, INotificationSession notificationSession)
        {
            var groupData = _jsonUtil.Deserialize<GroupData>(group.Data) ?? new GroupData();
            if (groupData.InCombat != true)
                throw new GroupNotInCombatException();

            groupData.InCombat = false;

            group.AddHistoryEntry(_groupHistoryUtil.CreateLogEndCombat(group));

            notificationSession.NotifyGroupChangeGroupData(group.Id, groupData);

            group.Data = _jsonUtil.Serialize(groupData);
        }

        public NhbkDate AddTimeAndNotify(Group group, NhbkDateOffset timeOffset, INotificationSession notificationSession)
        {
            var groupData = _jsonUtil.Deserialize<GroupData>(group.Data) ?? new GroupData();
            if (groupData.Date == null)
                throw new GroupDateNotSetException();

            groupData.Date.Add(timeOffset);

            group.AddHistoryEntry(_groupHistoryUtil.CreateLogAddTime(group, groupData.Date, timeOffset));
            notificationSession.NotifyGroupChangeGroupData(group.Id, groupData);

            group.Data = _jsonUtil.Serialize(groupData);

            return groupData.Date;
        }
    }
}