namespace Butterfly.Communication.Packets.Outgoing.Rooms.Session
{
    internal class FlatAccessibleComposer : ServerPacket
    {
        public FlatAccessibleComposer(string username)
            : base(ServerPacketHeader.ROOM_DOORBELL_ACCEPTED)
        {
            WriteString(username);
        }
    }
}
