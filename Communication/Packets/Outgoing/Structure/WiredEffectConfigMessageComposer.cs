namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class WiredEffectConfigMessageComposer : ServerPacket
    {
        public WiredEffectConfigMessageComposer()
            : base(ServerPacketHeader.WIRED_ACTION)
        {

        }
    }
}
