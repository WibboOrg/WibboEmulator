namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OpenHelpToolComposer : ServerPacket
    {
        public OpenHelpToolComposer()
            : base(ServerPacketHeader.OpenHelpToolMessageComposer)
        {

        }
    }
}
