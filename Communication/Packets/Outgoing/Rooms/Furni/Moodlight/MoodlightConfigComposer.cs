namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Moodlight
{
    internal class MoodlightConfigComposer : ServerPacket
    {
        public MoodlightConfigComposer()
            : base(ServerPacketHeader.ITEM_DIMMER_SETTINGS)
        {

        }
    }
}
