namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class FlatAccessDeniedComposer : ServerPacket
    {
        public FlatAccessDeniedComposer()
            : base(ServerPacketHeader.ROOM_DOORBELL_DENIED)
        {

        }
    }
}
