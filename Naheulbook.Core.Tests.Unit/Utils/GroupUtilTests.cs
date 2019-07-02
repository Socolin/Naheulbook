using System.Threading.Tasks;
using FluentAssertions;
using Naheulbook.Core.Models;
using Naheulbook.Core.Services;
using Naheulbook.Core.Utils;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Utils
{
    public class GroupUtilTests
    {
        private const string GroupDataJson = "some-group-data-json";
        private const string UpdatedGroupDataJson = "some-group-data-json";
        private const int GroupId = 8;

        private IJsonUtil _jsonUtil;
        private IChangeNotifier _changeNotifier;
        private IGroupHistoryUtil _groupHistoryUtil;
        private GroupUtil _util;

        [SetUp]
        public void SetUp()
        {
            _jsonUtil = Substitute.For<IJsonUtil>();
            _changeNotifier = Substitute.For<IChangeNotifier>();
            _groupHistoryUtil = Substitute.For<IGroupHistoryUtil>();

            _util = new GroupUtil(
                _jsonUtil,
                _changeNotifier,
                _groupHistoryUtil
            );
        }

        [Test]
        public async Task ApplyChangesAndNotifyAsync_WhenMankdebolIsSet_UpdateValue_LogInGroupHistory()
        {
            var group = new Group {Data = GroupDataJson};
            var groupData = new GroupData();
            var groupHistoryEntry = new GroupHistoryEntry();
            var request = new PatchGroupRequest
            {
                Mankdebol = 4
            };

            _groupHistoryUtil.CreateLogChangeMankdebol(group, null, 4)
                .Returns(groupHistoryEntry);
            _jsonUtil.Deserialize<GroupData>(GroupDataJson)
                .Returns(groupData);
            _jsonUtil.Serialize(groupData)
                .Returns(UpdatedGroupDataJson);

            await _util.ApplyChangesAndNotifyAsync(group, request);

            group.Data.Should().BeEquivalentTo(UpdatedGroupDataJson);
            groupData.Mankdebol.Should().Be(4);
            group.HistoryEntries.Should().Contain(groupHistoryEntry);
        }

        [Test]
        public async Task ApplyChangesAndNotifyAsync_WhenDebilibeukIsSet_UpdateValue_LogInGroupHistory()
        {
            var group = new Group {Data = GroupDataJson, Id = GroupId};
            var groupData = new GroupData();
            var groupHistoryEntry = new GroupHistoryEntry();
            var request = new PatchGroupRequest
            {
                Debilibeuk = 4
            };

            _groupHistoryUtil.CreateLogChangeDebilibeuk(group, null, 4)
                .Returns(groupHistoryEntry);
            _jsonUtil.Deserialize<GroupData>(GroupDataJson)
                .Returns(groupData);
            _jsonUtil.Serialize(groupData)
                .Returns(UpdatedGroupDataJson);

            await _util.ApplyChangesAndNotifyAsync(group, request);

            group.Data.Should().BeEquivalentTo(UpdatedGroupDataJson);
            groupData.Debilibeuk.Should().Be(4);
            group.HistoryEntries.Should().Contain(groupHistoryEntry);
        }

        [Test]
        public async Task ApplyChangesAndNotifyAsync_WhenDateIsSet_UpdateValue_LogInGroupHistory()
        {
            var group = new Group {Data = GroupDataJson, Id = GroupId};
            var groupData = new GroupData();
            var groupHistoryEntry = new GroupHistoryEntry();
            var request = new PatchGroupRequest
            {
                Date = new NhbkDateRequest()
                {
                    Day = 8
                }
            };

            _groupHistoryUtil.CreateLogChangeDate(group, Arg.Any<NhbkDate>(), Arg.Any<NhbkDate>())
                .Returns(groupHistoryEntry);
            _jsonUtil.Deserialize<GroupData>(GroupDataJson)
                .Returns(groupData);
            _jsonUtil.Serialize(groupData)
                .Returns(UpdatedGroupDataJson);

            await _util.ApplyChangesAndNotifyAsync(group, request);

            group.Data.Should().BeEquivalentTo(UpdatedGroupDataJson);
            groupData.Date.Day.Should().Be(8);
            group.HistoryEntries.Should().Contain(groupHistoryEntry);
        }

        [Test]
        public async Task ApplyChangesAndNotifyAsync_ShouldNotifyGroupDataChange()
        {
            var groupData = new GroupData();
            var group = new Group {Data = GroupDataJson, Id = GroupId};

            _jsonUtil.Deserialize<GroupData>(GroupDataJson)
                .Returns(groupData);

            await _util.ApplyChangesAndNotifyAsync(group, new PatchGroupRequest());

            await _changeNotifier.Received(1).NotifyGroupChangeGroupDataAsync(GroupId, groupData);
        }
    }
}