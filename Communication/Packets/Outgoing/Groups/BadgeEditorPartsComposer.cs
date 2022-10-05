namespace WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Games.Groups;

internal class BadgeEditorPartsComposer : ServerPacket
{
    public BadgeEditorPartsComposer(ICollection<GroupBadgeParts> bases, ICollection<GroupBadgeParts> symbols, ICollection<GroupColours> baseColours, ICollection<GroupColours> symbolColours,
      ICollection<GroupColours> backgroundColours)
        : base(ServerPacketHeader.GROUP_BADGE_PARTS)
    {
        this.WriteInteger(bases.Count);
        foreach (var part in bases)
        {
            this.WriteInteger(part.Id);
            this.WriteString(part.AssetOne);
            this.WriteString(part.AssetTwo);
        }

        this.WriteInteger(symbols.Count);
        foreach (var part in symbols)
        {
            this.WriteInteger(part.Id);
            this.WriteString(part.AssetOne);
            this.WriteString(part.AssetTwo);
        }

        this.WriteInteger(baseColours.Count);
        foreach (var colour in baseColours)
        {
            this.WriteInteger(colour.Id);
            this.WriteString(colour.Colour);
        }

        this.WriteInteger(symbolColours.Count);
        foreach (var colour in symbolColours)
        {
            this.WriteInteger(colour.Id);
            this.WriteString(colour.Colour);
        }

        this.WriteInteger(backgroundColours.Count);
        foreach (var colour in backgroundColours)
        {
            this.WriteInteger(colour.Id);
            this.WriteString(colour.Colour);
        }
    }
}
