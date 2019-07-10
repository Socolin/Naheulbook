using Naheulbook.Core.Notifications;

namespace Naheulbook.Web.Notifications
{
    public class NotificationPacket : INotificationPacket
    {
        public string GroupName { get; set; }
        public INotificationPacketPayload Payload { get; set; }
    }

    public class NotificationPacketPayload : INotificationPacketPayload
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Opcode { get; set; }
        public object Data { get; set; }
    }
}