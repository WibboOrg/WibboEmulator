using Butterfly.HabboHotel.Items;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Moodlight
{
    internal class MoodlightConfigMessageComposer : ServerPacket
    {
        public MoodlightConfigMessageComposer(MoodlightData MoodlightData)
            : base(ServerPacketHeader.ITEM_DIMMER_SETTINGS)
        {
            WriteInteger(MoodlightData.Presets.Count);
            WriteInteger(MoodlightData.CurrentPreset);

            int i = 1;
            foreach (MoodlightPreset Preset in MoodlightData.Presets)
            {
                WriteInteger(i);
                WriteInteger(Preset.BackgroundOnly ? 2 : 1);
                WriteString(Preset.ColorCode);
                WriteInteger(Preset.ColorIntensity);
                i++;
            }
        }
    }
}
