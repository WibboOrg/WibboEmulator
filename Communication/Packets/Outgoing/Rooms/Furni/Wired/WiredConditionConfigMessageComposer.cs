namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class WiredConditionConfigMessageComposer : ServerPacket
    {
        public WiredConditionConfigMessageComposer()
            : base(ServerPacketHeader.WIRED_CONDITION)
        {

        }
    }
}
