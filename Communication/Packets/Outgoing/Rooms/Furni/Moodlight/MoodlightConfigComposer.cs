namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;
using WibboEmulator.Games.Rooms.Moodlight;

internal class MoodlightConfigComposer : ServerPacket
{
    public MoodlightConfigComposer(MoodlightData MoodlightData)
        : base(ServerPacketHeader.ITEM_DIMMER_SETTINGS)
    {
        this.WriteInteger(MoodlightData.Presets.Count);
        this.WriteInteger(MoodlightData.CurrentPreset);

        var i = 0;
        foreach (var moodlightPreset in MoodlightData.Presets)
        {
            i++;
            this.WriteInteger(i);
            this.WriteInteger(int.Parse(WibboEnvironment.BoolToEnum(moodlightPreset.BackgroundOnly)) + 1);
            this.WriteString(moodlightPreset.ColorCode);
            this.WriteInteger(moodlightPreset.ColorIntensity);
        }
    }
}
