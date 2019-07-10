namespace Naheulbook.Core.Notifications
{
    public interface INotificationSessionFactory
    {
        INotificationSession CreateSession();
    }

    public class NotificationSessionFactory : INotificationSessionFactory
    {
        private readonly INotificationPacketBuilder _packetBuilder;
        private readonly INotificationSender _notificationSender;

        public NotificationSessionFactory(
            INotificationPacketBuilder packetBuilder,
            INotificationSender notificationSender
        )
        {
            _packetBuilder = packetBuilder;
            _notificationSender = notificationSender;
        }

        public INotificationSession CreateSession()
        {
            return new NotificationSession(_packetBuilder, _notificationSender);
        }
    }
}