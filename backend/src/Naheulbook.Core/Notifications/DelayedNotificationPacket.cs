namespace Naheulbook.Core.Notifications;

public class DelayedNotificationPacket(Func<INotificationPacket> buildPacket) : INotificationPacket
{
    private INotificationPacket? _builtPacket;

    public string GroupName
    {
        get
        {
            _builtPacket ??= buildPacket();
            return _builtPacket.GroupName;
        }
    }

    public INotificationPacketPayload Payload
    {
        get
        {
            _builtPacket ??= buildPacket();
            return _builtPacket.Payload;
        }
    }
}