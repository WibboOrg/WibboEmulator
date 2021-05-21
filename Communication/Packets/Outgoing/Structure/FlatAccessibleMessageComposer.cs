namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class FlatAccessibleMessageComposer : ServerPacket
    {
        public FlatAccessibleMessageComposer()
            : base(ServerPacketHeader.ROOM_DOORBELL_CLOSE)
        {

        }
    }
}
