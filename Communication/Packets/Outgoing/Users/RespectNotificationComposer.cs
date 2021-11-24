namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class RespectNotificationComposer : ServerPacket
    {
        public RespectNotificationComposer()
            : base(ServerPacketHeader.USER_RESPECT)
        {

        }
    }
}
