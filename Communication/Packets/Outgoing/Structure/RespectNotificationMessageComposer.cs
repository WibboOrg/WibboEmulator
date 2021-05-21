namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class RespectNotificationMessageComposer : ServerPacket
    {
        public RespectNotificationMessageComposer()
            : base(ServerPacketHeader.USER_RESPECT)
        {

        }
    }
}
