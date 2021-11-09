namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class FlatAccessDeniedMessageComposer : ServerPacket
    {
        public FlatAccessDeniedMessageComposer()
            : base(ServerPacketHeader.ROOM_DOORBELL_DENIED)
        {

        }
    }
}
