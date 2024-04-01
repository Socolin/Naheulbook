using Naheulbook.Core.Notifications;

namespace Naheulbook.Web.Notifications;

public class NotificationPacket(string groupName, INotificationPacketPayload payload) : INotificationPacket
{
    public string GroupName { get; } = groupName;
    public INotificationPacketPayload Payload { get; } = payload;
}

public class NotificationPacketPayload(int id, string type, string opcode, object data)
    : INotificationPacketPayload
{
    public int Id { get; } = id;
    public string Type { get; } = type;
    public string Opcode { get; } = opcode;
    public object Data { get; } = data;
}