namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Moodlight
{
    internal class MoodlightConfigMessageComposer : ServerPacket
    {
        public MoodlightConfigMessageComposer()
            : base(ServerPacketHeader.ITEM_DIMMER_SETTINGS)
        {

        }
    }
}
