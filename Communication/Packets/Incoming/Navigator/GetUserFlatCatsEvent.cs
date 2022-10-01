using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Navigator;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GetUserFlatCatsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            ICollection<SearchResultList> Categories = WibboEnvironment.GetGame().GetNavigator().GetFlatCategories();

            Session.SendPacket(new UserFlatCatsComposer(Categories, Session.GetUser().Rank));
        }
    }
}