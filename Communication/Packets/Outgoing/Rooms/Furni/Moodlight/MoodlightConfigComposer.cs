using Butterfly.Game.Rooms.Moodlight;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Moodlight
{
    internal class MoodlightConfigComposer : ServerPacket
    {
        public MoodlightConfigComposer(MoodlightData MoodlightData)
            : base(ServerPacketHeader.ITEM_DIMMER_SETTINGS)
        {
            this.WriteInteger(MoodlightData.Presets.Count);
            this.WriteInteger(MoodlightData.CurrentPreset);

            int i = 0;
            foreach (MoodlightPreset moodlightPreset in MoodlightData.Presets)
            {
                i++;
                this.WriteInteger(i);
                this.WriteInteger(int.Parse(ButterflyEnvironment.BoolToEnum(moodlightPreset.BackgroundOnly)) + 1);
                this.WriteString(moodlightPreset.ColorCode);
                this.WriteInteger(moodlightPreset.ColorIntensity);
            }
        }
    }
}
