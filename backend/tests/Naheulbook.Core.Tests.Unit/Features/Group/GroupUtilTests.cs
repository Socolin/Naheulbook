using FluentAssertions;
using Naheulbook.Core.Features.Group;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.Utils;
using NSubstitute;
using NUnit.Framework;

namespace Naheulbook.Core.Tests.Unit.Features.Group;

public class GroupUtilTests
{
    private const string GroupDataJson = "some-group-data-json";
    private const string UpdatedGroupDataJson = "some-group-data-json";
    private const int GroupId = 8;

    private IJsonUtil _jsonUtil;
    private IGroupHistoryUtil _groupHistoryUtil;
    private GroupUtil _util;
    private INotificationSession _notificationSession;

    [SetUp]
    public void SetUp()
    {
        _jsonUtil = Substitute.For<IJsonUtil>();
        _groupHistoryUtil = Substitute.For<IGroupHistoryUtil>();

        _util = new GroupUtil(
            _jsonUtil,
            _groupHistoryUtil
        );

        _notificationSession = Substitute.For<INotificationSession>();
    }

    [Test]
    public void ApplyChangesAndNotifyAsync_WhenMankdebolIsSet_UpdateValue_LogInGroupHistory()
    {
        var group = new GroupEntity {Data = GroupDataJson};
        var groupData = new GroupData();
        var groupHistoryEntry = new GroupHistoryEntryEntity();
        var request = new PatchGroupRequest
        {
            Mankdebol = 4,
        };

        _groupHistoryUtil.CreateLogChangeMankdebol(group, null, 4)
            .Returns(groupHistoryEntry);
        _jsonUtil.Deserialize<GroupData>(GroupDataJson)
            .Returns(groupData);
        _jsonUtil.Serialize(groupData)
            .Returns(UpdatedGroupDataJson);

        _util.ApplyChangesAndNotify(group, request, _notificationSession);

        group.Data.Should().BeEquivalentTo(UpdatedGroupDataJson);
        groupData.Mankdebol.Should().Be(4);
        group.HistoryEntries.Should().Contain(groupHistoryEntry);
    }

    [Test]
    public void ApplyChangesAndNotifyAsync_WhenDebilibeukIsSet_UpdateValue_LogInGroupHistory()
    {
        var group = new GroupEntity {Data = GroupDataJson, Id = GroupId};
        var groupData = new GroupData();
        var groupHistoryEntry = new GroupHistoryEntryEntity();
        var request = new PatchGroupRequest
        {
            Debilibeuk = 4,
        };

        _groupHistoryUtil.CreateLogChangeDebilibeuk(group, null, 4)
            .Returns(groupHistoryEntry);
        _jsonUtil.Deserialize<GroupData>(GroupDataJson)
            .Returns(groupData);
        _jsonUtil.Serialize(groupData)
            .Returns(UpdatedGroupDataJson);

        _util.ApplyChangesAndNotify(group, request, _notificationSession);

        group.Data.Should().BeEquivalentTo(UpdatedGroupDataJson);
        groupData.Debilibeuk.Should().Be(4);
        group.HistoryEntries.Should().Contain(groupHistoryEntry);
    }

    [Test]
    public void ApplyChangesAndNotifyAsync_WhenDateIsSet_UpdateValue_LogInGroupHistory()
    {
        var group = new GroupEntity {Data = GroupDataJson, Id = GroupId};
        var groupData = new GroupData();
        var groupHistoryEntry = new GroupHistoryEntryEntity();
        var request = new PatchGroupRequest
        {
            Date = new NhbkDateRequest()
            {
                Day = 8,
            },
        };

        _groupHistoryUtil.CreateLogChangeDate(group, Arg.Any<NhbkDate>(), Arg.Any<NhbkDate>())
            .Returns(groupHistoryEntry);
        _jsonUtil.Deserialize<GroupData>(GroupDataJson)
            .Returns(groupData);
        _jsonUtil.Serialize(groupData)
            .Returns(UpdatedGroupDataJson);

        _util.ApplyChangesAndNotify(group, request, _notificationSession);

        group.Data.Should().BeEquivalentTo(UpdatedGroupDataJson);
        groupData.Date!.Day.Should().Be(8);
        group.HistoryEntries.Should().Contain(groupHistoryEntry);
    }

    [Test]
    public void ApplyChangesAndNotifyAsync_ShouldNotifyGroupDataChange()
    {
        var groupData = new GroupData();
        var group = new GroupEntity {Data = GroupDataJson, Id = GroupId};

        _jsonUtil.Deserialize<GroupData>(GroupDataJson)
            .Returns(groupData);

        _util.ApplyChangesAndNotify(group, new PatchGroupRequest(), _notificationSession);


        _notificationSession.Received(1).NotifyGroupChangeGroupData(GroupId, groupData);
    }
}