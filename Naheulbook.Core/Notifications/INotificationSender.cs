using System.Collections.Generic;
using System.Threading.Tasks;

namespace Naheulbook.Core.Notifications;

public interface INotificationSender
{
    Task SendPacketsAsync(IEnumerable<INotificationPacket> packets);
}