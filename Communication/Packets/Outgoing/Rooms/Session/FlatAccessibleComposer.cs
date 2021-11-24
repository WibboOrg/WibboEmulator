namespace Butterfly.Communication.Packets.Outgoing.Rooms.Session
{
    internal class FlatAccessibleComposer : ServerPacket
    {
        public FlatAccessibleComposer()
            : base(ServerPacketHeader.ROOM_DOORBELL_CLOSE)
        {

        }
    }
}
