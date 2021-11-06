namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class FlatAccessDeniedMessageComposer : ServerPacket
    {
        public FlatAccessDeniedMessageComposer(string Username)
            : base(ServerPacketHeader.ROOM_DOORBELL_DENIED)
        {
            WriteString(Username);
        }
    }
}
