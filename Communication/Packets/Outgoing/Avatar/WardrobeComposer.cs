namespace WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Games.Users.Wardrobes;

internal class WardrobeComposer : ServerPacket
{
    public WardrobeComposer(Dictionary<int, Wardrobe> wardrobes)
        : base(ServerPacketHeader.USER_OUTFITS)
    {
        this.WriteInteger(1);

        this.WriteInteger(wardrobes.Count);
        foreach (var wardrobe in wardrobes.Values)
        {
            this.WriteInteger(wardrobe.SlotId);
            this.WriteString(wardrobe.Look);
            this.WriteString(wardrobe.Gender);
        }
    }
}
