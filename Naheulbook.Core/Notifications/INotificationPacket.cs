namespace Naheulbook.Core.Notifications
{
    public interface INotificationPacket
    {
        string GroupName { get; set; }
        INotificationPacketPayload Payload { get; set; }
    }

    public interface INotificationPacketPayload
    {
    }
}