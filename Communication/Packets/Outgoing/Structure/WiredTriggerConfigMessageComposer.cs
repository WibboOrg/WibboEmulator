namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class WiredTriggerConfigMessageComposer : ServerPacket
    {
        public WiredTriggerConfigMessageComposer()
            : base(ServerPacketHeader.WIRED_TRIGGER)
        {

        }
    }
}
