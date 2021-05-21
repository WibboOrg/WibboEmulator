namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class WiredConditionConfigMessageComposer : ServerPacket
    {
        public WiredConditionConfigMessageComposer()
            : base(ServerPacketHeader.WIRED_CONDITION)
        {

        }
    }
}
