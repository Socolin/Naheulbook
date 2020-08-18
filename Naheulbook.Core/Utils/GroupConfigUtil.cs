using System;
using Naheulbook.Core.Notifications;
using Naheulbook.Data.Models;
using Naheulbook.Requests.Requests;
using Naheulbook.Shared.TransientModels;
using Naheulbook.Shared.Utils;

namespace Naheulbook.Core.Utils
{
    public interface IGroupConfigUtil
    {
        void ApplyChangesAndNotify(Group group, PatchGroupConfigRequest request, INotificationSession notificationSession);
    }

    public class GroupConfigUtil : IGroupConfigUtil
    {
        private readonly IJsonUtil _jsonUtil;

        public GroupConfigUtil(IJsonUtil jsonUtil)
        {
            _jsonUtil = jsonUtil;
        }

        public void ApplyChangesAndNotify(Group group, PatchGroupConfigRequest request, INotificationSession notificationSession)
        {
            var config = _jsonUtil.DeserializeOrCreate<GroupConfig>(group.Config);

            if (request.AllowPlayersToAddObject.HasValue)
                config.AllowPlayersToAddObject = request.AllowPlayersToAddObject.Value;
            if (request.AllowPlayersToSeeSkillGmDetails.HasValue)
                config.AllowPlayersToSeeSkillGmDetails = request.AllowPlayersToSeeSkillGmDetails.Value;
            if (request.AllowPlayersToSeeGemPriceWhenIdentified.HasValue)
                config.AllowPlayersToSeeGemPriceWhenIdentified = request.AllowPlayersToSeeGemPriceWhenIdentified.Value;

            group.Config = _jsonUtil.SerializeNonNull(config);

            notificationSession.NotifyGroupChangeConfig(group.Id, config);
        }
    }
}