namespace Butterfly.Communication.Packets.Outgoing.Rooms.Session
{
    internal class FlatAccessibleMessageComposer : ServerPacket
    {
        public FlatAccessibleMessageComposer()
            : base(ServerPacketHeader.ROOM_DOORBELL_CLOSE)
        {

        }
    }
}
