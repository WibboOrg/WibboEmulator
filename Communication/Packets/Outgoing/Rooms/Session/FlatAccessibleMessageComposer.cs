namespace Butterfly.Communication.Packets.Outgoing.Rooms.Session
{
    internal class FlatAccessibleMessageComposer : ServerPacket
    {
        public FlatAccessibleMessageComposer(string Username)
            : base(ServerPacketHeader.ROOM_DOORBELL_CLOSE)
        {
            WriteString(Username);
        }
    }
}
