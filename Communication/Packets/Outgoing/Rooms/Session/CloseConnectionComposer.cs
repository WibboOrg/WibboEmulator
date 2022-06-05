namespace Wibbo.Communication.Packets.Outgoing.Rooms.Session
{
    internal class CloseConnectionComposer : ServerPacket
    {
        public CloseConnectionComposer()
            : base(ServerPacketHeader.DESKTOP_VIEW)
        {

        }
    }
}
