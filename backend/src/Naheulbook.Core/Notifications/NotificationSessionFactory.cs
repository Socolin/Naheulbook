namespace Naheulbook.Core.Notifications;

public interface INotificationSessionFactory
{
    INotificationSession CreateSession();
}

public class NotificationSessionFactory(
    INotificationPacketBuilder packetBuilder,
    INotificationSender notificationSender
) : INotificationSessionFactory
{
    public INotificationSession CreateSession()
    {
        return new NotificationSession(packetBuilder, notificationSender);
    }
}