using Naheulbook.Core.Notifications;
using NSubstitute;

namespace Naheulbook.Core.Tests.Unit.TestUtils
{
    public class FakeNotificationSessionFactory : INotificationSessionFactory
    {
        public INotificationSession NotificationSession { get; } = Substitute.For<INotificationSession>();

        public INotificationSession CreateSession()
        {
            return NotificationSession;
        }
    }
}