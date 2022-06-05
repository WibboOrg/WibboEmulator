using Butterfly.Game.Moderation;

namespace Butterfly.Communication.Packets.Outgoing
{
    internal class CfhTopicsInitComposer : ServerPacket
    {
        public CfhTopicsInitComposer(Dictionary<string, List<ModerationPresetActions>> UserActionPresets)
            : base(ServerPacketHeader.CFH_TOPICS)
        {

            this.WriteInteger(UserActionPresets.Count);
            foreach (KeyValuePair<string, List<ModerationPresetActions>> Cat in UserActionPresets.ToList())
            {
                this.WriteString(Cat.Key);
                this.WriteInteger(Cat.Value.Count);
                foreach (ModerationPresetActions Preset in Cat.Value.ToList())
                {
                    this.WriteString(Preset.Caption);
                    this.WriteInteger(Preset.Id);
                    this.WriteString(Preset.Type);
                }
            }
        }
    }
}
