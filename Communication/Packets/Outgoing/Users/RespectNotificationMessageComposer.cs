namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class RespectNotificationMessageComposer : ServerPacket
    {
        public RespectNotificationMessageComposer()
            : base(ServerPacketHeader.USER_RESPECT)
        {

        }
    }
}
