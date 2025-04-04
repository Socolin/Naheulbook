using Naheulbook.Core.Notifications;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Features.Group;

public interface IGroupConfigUtil
{
    void ApplyChangesAndNotify(GroupEntity group, PatchGroupConfigRequest request, INotificationSession notificationSession);
}

public class GroupConfigUtil(IJsonUtil jsonUtil) : IGroupConfigUtil
{
    public void ApplyChangesAndNotify(GroupEntity group, PatchGroupConfigRequest request, INotificationSession notificationSession)
    {
        var config = jsonUtil.DeserializeOrCreate<GroupConfig>(group.Config);

        if (request.AllowPlayersToAddObject.HasValue)
            config.AllowPlayersToAddObject = request.AllowPlayersToAddObject.Value;
        if (request.AllowPlayersToSeeSkillGmDetails.HasValue)
            config.AllowPlayersToSeeSkillGmDetails = request.AllowPlayersToSeeSkillGmDetails.Value;
        if (request.AllowPlayersToSeeGemPriceWhenIdentified.HasValue)
            config.AllowPlayersToSeeGemPriceWhenIdentified = request.AllowPlayersToSeeGemPriceWhenIdentified.Value;

        group.Config = jsonUtil.SerializeNonNull(config);

        notificationSession.NotifyGroupChangeConfig(group.Id, config);
    }
}