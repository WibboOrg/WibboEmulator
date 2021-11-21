using Butterfly.Game.Guilds;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class BadgeEditorPartsComposer : ServerPacket
    {
        public BadgeEditorPartsComposer(ICollection<GuildBadgePart> bases, ICollection<GuildBadgePart> symbols, ICollection<GuildColour> baseColours, ICollection<GuildColour> symbolColours,
          ICollection<GuildColour> backgroundColours)
            : base(ServerPacketHeader.GROUP_BADGE_PARTS)
        {
            this.WriteInteger(bases.Count);
            foreach (GuildBadgePart part in bases)
            {
                this.WriteInteger(part.Id);
                this.WriteString(part.AssetOne);
                this.WriteString(part.AssetTwo);
            }

            this.WriteInteger(symbols.Count);
            foreach (GuildBadgePart part in symbols)
            {
                this.WriteInteger(part.Id);
                this.WriteString(part.AssetOne);
                this.WriteString(part.AssetTwo);
            }

            this.WriteInteger(baseColours.Count);
            foreach (GuildColour colour in baseColours)
            {
                this.WriteInteger(colour.Id);
                this.WriteString(colour.Colour);
            }

            this.WriteInteger(symbolColours.Count);
            foreach (GuildColour colour in symbolColours)
            {
                this.WriteInteger(colour.Id);
                this.WriteString(colour.Colour);
            }

            this.WriteInteger(backgroundColours.Count);
            foreach (GuildColour colour in backgroundColours)
            {
                this.WriteInteger(colour.Id);
                this.WriteString(colour.Colour);
            }
        }
    }
}
