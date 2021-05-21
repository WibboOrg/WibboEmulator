namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FlatAccessDeniedMessageComposer : ServerPacket
    {
        public FlatAccessDeniedMessageComposer()
            : base(ServerPacketHeader.ROOM_DOORBELL_DENIED)
        {

        }
    }
}
