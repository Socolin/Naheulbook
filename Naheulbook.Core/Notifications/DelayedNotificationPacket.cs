using System;

namespace Naheulbook.Core.Notifications
{
    public class DelayedNotificationPacket : INotificationPacket
    {
        private readonly Func<INotificationPacket> _buildPacket;
        private INotificationPacket? _builtPacket;

        public DelayedNotificationPacket(Func<INotificationPacket> buildPacket)
        {
            _buildPacket = buildPacket;
        }

        public string GroupName
        {
            get
            {
                _builtPacket ??= _buildPacket();
                return _builtPacket.GroupName;
            }
        }

        public INotificationPacketPayload Payload
        {
            get
            {
                _builtPacket ??= _buildPacket();
                return _builtPacket.Payload;
            }
        }
    }
}