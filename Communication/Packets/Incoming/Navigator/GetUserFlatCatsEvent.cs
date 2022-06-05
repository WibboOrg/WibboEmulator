using Wibbo.Communication.Packets.Outgoing.Navigator;
using Wibbo.Game.Clients;
using Wibbo.Game.Navigator;

namespace Wibbo.Communication.Packets.Incoming.Structure
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