namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OpenHelpToolComposer : ServerPacket
    {
        public OpenHelpToolComposer(int type)
            : base(ServerPacketHeader.OpenHelpToolMessageComposer)
        {
            WriteInteger(type);
        }
    }
}
