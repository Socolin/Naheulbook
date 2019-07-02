using System.Collections.Generic;
using System.Threading.Tasks;
using Naheulbook.Core.Exceptions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface IGroupUtil
    {
        Task ApplyChangesAndNotifyAsync(Group group, PatchGroupRequest request);
        Task StartCombatAsync(Group group);
        Task EndCombatAsync(Group group);
    }

    public class GroupUtil : IGroupUtil
    {
        private readonly IJsonUtil _jsonUtil;
        private readonly IChangeNotifier _changeNotifier;
        private readonly IGroupHistoryUtil _groupHistoryUtil;

        public GroupUtil(IJsonUtil jsonUtil, IChangeNotifier changeNotifier, IGroupHistoryUtil groupHistoryUtil)
        {
            _jsonUtil = jsonUtil;
            _changeNotifier = changeNotifier;
            _groupHistoryUtil = groupHistoryUtil;
        }

        public async Task ApplyChangesAndNotifyAsync(Group group, PatchGroupRequest request)
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

            await _changeNotifier.NotifyGroupChangeGroupDataAsync(group.Id, groupData);

            group.Data = _jsonUtil.Serialize(groupData);
        }

        public async Task StartCombatAsync(Group group)
        {
            var groupData = _jsonUtil.Deserialize<GroupData>(group.Data) ?? new GroupData();
            if (groupData.InCombat == true)
                throw new GroupAlreadyInCombatException();

            groupData.InCombat = true;

            group.Loots = group.Loots ?? new List<Loot>();
            var loot = new Loot
            {
                Name = "Combat",
                GroupId = group.Id
            };
            group.CombatLoot = loot;

            group.AddHistoryEntry(_groupHistoryUtil.CreateLogStartCombat(group));

            await _changeNotifier.NotifyGroupChangeGroupDataAsync(group.Id, groupData);
            await _changeNotifier.NotifyGroupAddLoot(group.Id, loot);

            group.Data = _jsonUtil.Serialize(groupData);
        }

        public async Task EndCombatAsync(Group group)
        {
            var groupData = _jsonUtil.Deserialize<GroupData>(group.Data) ?? new GroupData();
            if (groupData.InCombat != true)
                throw new GroupNotInCombatException();

            groupData.InCombat = false;

            group.AddHistoryEntry(_groupHistoryUtil.CreateLogEndCombat(group));

            await _changeNotifier.NotifyGroupChangeGroupDataAsync(group.Id, groupData);

            group.Data = _jsonUtil.Serialize(groupData);
        }
    }
}