using Butterfly.Game.Navigator;

namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class UserFlatCatsComposer : ServerPacket
    {
        public UserFlatCatsComposer(ICollection<SearchResultList> Categories, int Rank)
            : base(ServerPacketHeader.NAVIGATOR_CATEGORIES)
        {
            this.WriteInteger(Categories.Count);
            foreach (SearchResultList Cat in Categories)
            {
                this.WriteInteger(Cat.Id);
                this.WriteString(Cat.PublicName);
                this.WriteBoolean(Cat.RequiredRank <= Rank);
                this.WriteBoolean(false);
                this.WriteString("");
                this.WriteString("");
                this.WriteBoolean(false);
            }
        }
    }
}
