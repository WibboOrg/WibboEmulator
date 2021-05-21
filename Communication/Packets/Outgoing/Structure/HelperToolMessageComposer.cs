namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class HelperToolMessageComposer : ServerPacket
    {
        public HelperToolMessageComposer()
            : base(ServerPacketHeader.HelperToolMessageComposer)
        {

        }
    }
}
