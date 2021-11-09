namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OpenHelpToolMessageComposer : ServerPacket
    {
        public OpenHelpToolMessageComposer()
            : base(ServerPacketHeader.OpenHelpToolMessageComposer)
        {

        }
    }
}
