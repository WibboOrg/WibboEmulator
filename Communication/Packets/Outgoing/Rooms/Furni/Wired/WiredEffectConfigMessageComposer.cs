namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class WiredEffectConfigMessageComposer : ServerPacket
    {
        public WiredEffectConfigMessageComposer()
            : base(ServerPacketHeader.WIRED_ACTION)
        {

        }
    }
}
