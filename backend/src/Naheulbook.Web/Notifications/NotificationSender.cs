using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Naheulbook.Core.Notifications;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Hubs;

namespace Naheulbook.Web.Notifications;

public class NotificationSender(
    IHubContext<ChangeNotifierHub> hubContext,
    IJsonUtil jsonUtil
) : INotificationSender
{
    public async Task SendPacketsAsync(IEnumerable<INotificationPacket> packets)
    {
        foreach (var notificationPacket in packets)
        {
            await hubContext.Clients.Group(notificationPacket.GroupName)
                .SendAsync("event", jsonUtil.Serialize(notificationPacket.Payload));
        }
    }
}