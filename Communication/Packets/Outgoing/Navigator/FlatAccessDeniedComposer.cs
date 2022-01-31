namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class FlatAccessDeniedComposer : ServerPacket
    {
        public FlatAccessDeniedComposer(string username)
            : base(ServerPacketHeader.ROOM_DOORBELL_REJECTED)
        {
            if (username != null)
                WriteString(username);
        }
    }
}
