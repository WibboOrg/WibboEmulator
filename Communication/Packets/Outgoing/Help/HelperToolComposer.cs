namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class HelperToolComposer : ServerPacket
    {
        public HelperToolComposer()
            : base(ServerPacketHeader.HelperToolMessageComposer)
        {

        }
    }
}
