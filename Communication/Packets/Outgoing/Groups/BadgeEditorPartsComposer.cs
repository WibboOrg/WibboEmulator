using Butterfly.Game.Guilds;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Groups
{
    internal class BadgeEditorPartsComposer : ServerPacket
    {
        public BadgeEditorPartsComposer(ICollection<GroupBadgePart> bases, ICollection<GroupBadgePart> symbols, ICollection<GroupColour> baseColours, ICollection<GroupColour> symbolColours,
          ICollection<GroupColour> backgroundColours)
            : base(ServerPacketHeader.GROUP_BADGE_PARTS)
        {
            this.WriteInteger(bases.Count);
            foreach (GroupBadgePart part in bases)
            {
                this.WriteInteger(part.Id);
                this.WriteString(part.AssetOne);
                this.WriteString(part.AssetTwo);
            }

            this.WriteInteger(symbols.Count);
            foreach (GroupBadgePart part in symbols)
            {
                this.WriteInteger(part.Id);
                this.WriteString(part.AssetOne);
                this.WriteString(part.AssetTwo);
            }

            this.WriteInteger(baseColours.Count);
            foreach (GroupColour colour in baseColours)
            {
                this.WriteInteger(colour.Id);
                this.WriteString(colour.Colour);
            }

            this.WriteInteger(symbolColours.Count);
            foreach (GroupColour colour in symbolColours)
            {
                this.WriteInteger(colour.Id);
                this.WriteString(colour.Colour);
            }

            this.WriteInteger(backgroundColours.Count);
            foreach (GroupColour colour in backgroundColours)
            {
                this.WriteInteger(colour.Id);
                this.WriteString(colour.Colour);
            }
        }
    }
}
