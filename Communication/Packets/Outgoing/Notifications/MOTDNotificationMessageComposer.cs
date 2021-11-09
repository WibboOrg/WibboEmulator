namespace Butterfly.Communication.Packets.Outgoing.Notifications
{
    internal class MOTDNotificationMessageComposer : ServerPacket
    {
        public MOTDNotificationMessageComposer()
            : base(ServerPacketHeader.GENERIC_ALERT_MESSAGES)
        {

        }
    }
}
