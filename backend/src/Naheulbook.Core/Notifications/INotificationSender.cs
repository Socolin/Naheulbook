namespace Naheulbook.Core.Notifications;

public interface INotificationSender
{
    Task SendPacketsAsync(IEnumerable<INotificationPacket> packets);
}