using Naheulbook.Core.Notifications;

namespace Naheulbook.Web.Notifications
{
    public class NotificationPacket : INotificationPacket
    {
        public string GroupName { get; }
        public INotificationPacketPayload Payload { get; }

        public NotificationPacket(string groupName, INotificationPacketPayload payload)
        {
            GroupName = groupName;
            Payload = payload;
        }
    }

    public class NotificationPacketPayload : INotificationPacketPayload
    {
        public int Id { get; }
        public string Type { get; }
        public string Opcode { get; }
        public object Data { get; }

        public NotificationPacketPayload(int id, string type, string opcode, object data)
        {
            Id = id;
            Type = type;
            Opcode = opcode;
            Data = data;
        }
    }
}