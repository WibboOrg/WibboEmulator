namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Wired
{
    internal class WiredTriggerConfigMessageComposer : ServerPacket
    {
        public WiredTriggerConfigMessageComposer()
            : base(ServerPacketHeader.WIRED_TRIGGER)
        {

        }
    }
}
