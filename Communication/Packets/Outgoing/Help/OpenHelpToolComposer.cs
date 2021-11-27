namespace Butterfly.Communication.Packets.Outgoing.Help
{
    internal class OpenHelpToolComposer : ServerPacket
    {
        public OpenHelpToolComposer(int type)
            : base(ServerPacketHeader.CFH_PENDING_CALLS)
        {
            WriteInteger(type);
        }
    }
}
