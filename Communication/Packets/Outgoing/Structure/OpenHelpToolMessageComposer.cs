namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class OpenHelpToolMessageComposer : ServerPacket
    {
        public OpenHelpToolMessageComposer()
            : base(ServerPacketHeader.OpenHelpToolMessageComposer)
        {

        }
    }
}
