using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Navigator;

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