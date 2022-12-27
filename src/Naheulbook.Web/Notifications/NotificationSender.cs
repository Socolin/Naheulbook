using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Naheulbook.Core.Notifications;
using Naheulbook.Shared.Utils;
using Naheulbook.Web.Hubs;

namespace Naheulbook.Web.Notifications;

public class NotificationSender : INotificationSender
{
    private readonly IHubContext<ChangeNotifierHub> _hubContext;
    private readonly IJsonUtil _jsonUtil;

    public NotificationSender(
        IHubContext<ChangeNotifierHub> hubContext,
        IJsonUtil jsonUtil
    )
    {
        _hubContext = hubContext;
        _jsonUtil = jsonUtil;
    }

    public async Task SendPacketsAsync(IEnumerable<INotificationPacket> packets)
    {
        foreach (var notificationPacket in packets)
        {
            await _hubContext.Clients.Group(notificationPacket.GroupName)
                .SendAsync("event", _jsonUtil.Serialize(notificationPacket.Payload));
        }
    }
}