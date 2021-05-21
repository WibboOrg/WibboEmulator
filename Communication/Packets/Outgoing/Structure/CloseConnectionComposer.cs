namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class CloseConnectionComposer : ServerPacket
    {
        public CloseConnectionComposer()
            : base(ServerPacketHeader.DESKTOP_VIEW)
        {

        }
    }
}
