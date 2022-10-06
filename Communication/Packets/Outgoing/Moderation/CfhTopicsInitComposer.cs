namespace WibboEmulator.Communication.Packets.Outgoing.Moderation;
using WibboEmulator.Games.Moderation;

internal class CfhTopicsInitComposer : ServerPacket
{
    public CfhTopicsInitComposer(Dictionary<string, List<ModerationPresetActions>> userActionPresets)
        : base(ServerPacketHeader.CFH_TOPICS)
    {

        this.WriteInteger(userActionPresets.Count);
        foreach (var cat in userActionPresets.ToList())
        {
            this.WriteString(cat.Key);
            this.WriteInteger(cat.Value.Count);
            foreach (var preset in cat.Value.ToList())
            {
                this.WriteString(preset.Caption);
                this.WriteInteger(preset.Id);
                this.WriteString(preset.Type);
            }
        }
    }
}
