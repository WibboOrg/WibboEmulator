namespace Butterfly.Communication.Packets.Outgoing.Structure
{
    internal class MoodlightConfigMessageComposer : ServerPacket
    {
        public MoodlightConfigMessageComposer()
            : base(ServerPacketHeader.ITEM_DIMMER_SETTINGS)
        {

        }
    }
}
