namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class MOTDNotificationMessageComposer : ServerPacket
    {
        public MOTDNotificationMessageComposer()
            : base(ServerPacketHeader.GENERIC_ALERT_MESSAGES)
        {

        }
    }
}
