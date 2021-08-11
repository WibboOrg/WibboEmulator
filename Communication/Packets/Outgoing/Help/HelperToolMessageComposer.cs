namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class HelperToolMessageComposer : ServerPacket
    {
        public HelperToolMessageComposer()
            : base(ServerPacketHeader.HelperToolMessageComposer)
        {

        }
    }
}
