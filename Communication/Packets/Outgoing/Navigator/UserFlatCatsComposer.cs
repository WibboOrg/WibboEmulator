namespace WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.Navigator;

internal class UserFlatCatsComposer : ServerPacket
{
    public UserFlatCatsComposer(ICollection<SearchResultList> categories, int rank)
        : base(ServerPacketHeader.NAVIGATOR_CATEGORIES)
    {
        this.WriteInteger(categories.Count);
        foreach (var cat in categories)
        {
            this.WriteInteger(cat.Id);
            this.WriteString(cat.PublicName);
            this.WriteBoolean(cat.RequiredRank <= rank);
            this.WriteBoolean(false);
            this.WriteString("");
            this.WriteString("");
            this.WriteBoolean(false);
        }
    }
}
