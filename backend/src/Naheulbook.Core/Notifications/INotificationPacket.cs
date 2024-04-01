namespace Naheulbook.Core.Notifications;

public interface INotificationPacket
{
    string GroupName { get; }
    INotificationPacketPayload Payload { get; }
}

public interface INotificationPacketPayload;