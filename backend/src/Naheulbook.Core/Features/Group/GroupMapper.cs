using Naheulbook.Core.Features.Shared;
using Naheulbook.Data.EntityFrameworkCore.Entities;
using Naheulbook.Responses.Responses;
using Naheulbook.Shared.TransientModels;
using Newtonsoft.Json.Linq;

namespace Naheulbook.Core.Features.Group;

public interface IGroupMapper
{
    GroupResponse ToResponse(GroupEntity group);
    GroupSummaryResponse ToSummaryResponse(GroupEntity group);
    CharacterGroupResponse ToCharacterGroupResponse(GroupEntity group);
    GroupHistoryEntryResponse ToResponse(GroupHistoryEntryEntity historyEntry);

    DeleteInviteResponse ToDeleteInviteResponse(GroupInviteEntity groupInvite);
    GroupInviteResponse ToInviteResponse(GroupInviteEntity groupInvite);
    GroupGroupInviteResponse ToGroupInviteResponse(GroupInviteEntity groupInvite);
    CharacterGroupInviteResponse ToCharacterInviteResponse(GroupInviteEntity groupInvite);
}

public class GroupMapper : IGroupMapper
{
    public GroupResponse ToResponse(GroupEntity group)
    {
        return new GroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Data = MapperHelpers.FromJson<JObject>(group.Data) ?? new JObject(),
            Config = MapperHelpers.FromJsonNotNull<GroupConfig>(group.Config),
            CharacterIds = group.Characters.Select(c => c.Id).ToList(),
            Invites = group.Invites.Select(ToGroupInviteResponse).ToList(),
        };
    }

    public GroupSummaryResponse ToSummaryResponse(GroupEntity group)
    {
        return new GroupSummaryResponse
        {
            Id = group.Id,
            Name = group.Name,
            CharacterCount = group.Characters.Count,
        };
    }

    public CharacterGroupResponse ToCharacterGroupResponse(GroupEntity group)
    {
        return new CharacterGroupResponse
        {
            Id = group.Id,
            Name = group.Name,
            Config = MapperHelpers.FromJsonNotNull<GroupConfig>(group.Config),
        };
    }

    public GroupHistoryEntryResponse ToResponse(GroupHistoryEntryEntity historyEntry)
    {
        return new GroupHistoryEntryResponse
        {
            Id = historyEntry.Id,
            Action = historyEntry.Action,
            Data = MapperHelpers.FromJson<JObject>(historyEntry.Data) ?? new JObject(),
            Date = MapperHelpers.ToUtcIsoString(historyEntry.Date),
            Gm = historyEntry.Gm,
        };
    }

    public DeleteInviteResponse ToDeleteInviteResponse(GroupInviteEntity groupInvite)
    {
        return new DeleteInviteResponse
        {
            GroupId = groupInvite.GroupId,
            CharacterId = groupInvite.CharacterId,
            FromGroup = groupInvite.FromGroup,
        };
    }

    public GroupInviteResponse ToInviteResponse(GroupInviteEntity groupInvite)
    {
        return new GroupInviteResponse
        {
            Id = groupInvite.Character.Id,
            Name = groupInvite.Character.Name,
            FromGroup = groupInvite.FromGroup,
            Group = ToCharacterGroupResponse(groupInvite.Group),
            OriginName = groupInvite.Character.Origin.Name,
            JobNames = groupInvite.Character.Jobs.Select(j => j.Job.Name).ToList(),
        };
    }

    public GroupGroupInviteResponse ToGroupInviteResponse(GroupInviteEntity groupInvite)
    {
        return new GroupGroupInviteResponse
        {
            Id = groupInvite.Character.Id,
            Name = groupInvite.Character.Name,
            OriginName = groupInvite.Character.Origin.Name,
            JobNames = groupInvite.Character.Jobs.Select(j => j.Job.Name).ToList(),
            FromGroup = groupInvite.FromGroup,
        };
    }

    public CharacterGroupInviteResponse ToCharacterInviteResponse(GroupInviteEntity groupInvite)
    {
        return new CharacterGroupInviteResponse
        {
            GroupName = groupInvite.Group.Name,
            GroupId = groupInvite.Group.Id,
            Config = MapperHelpers.FromJsonNotNull<GroupConfig>(groupInvite.Group.Config),
        };
    }
}
