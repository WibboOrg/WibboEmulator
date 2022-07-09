using Wibbo.Communication.Packets.Outgoing.Navigator.New;

using Wibbo.Game.Clients;
using Wibbo.Game.Navigator;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class NavigatorSearchEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            string Category = Packet.PopString();
            string Search = Packet.PopString();

            ICollection<SearchResultList> Categories = new List<SearchResultList>();

            if (!string.IsNullOrEmpty(Search))
            {
                if (WibboEnvironment.GetGame().GetNavigator().TryGetSearchResultList(0, out SearchResultList QueryResult))
                {
                    Categories.Add(QueryResult);
                }
            }
            else
            {
                Categories = WibboEnvironment.GetGame().GetNavigator().GetCategorysForSearch(Category);
                if (Categories.Count == 0)
                {
                    //Are we going in deep?!
                    Categories = WibboEnvironment.GetGame().GetNavigator().GetResultByIdentifier(Category);
                    if (Categories.Count > 0)
                    {
                        Session.SendPacket(new NavigatorSearchResultSetComposer(Category, Search, Categories, Session, 2, 50));
                        return;
                    }
                }
            }

            Session.SendPacket(new NavigatorSearchResultSetComposer(Category, Search, Categories, Session));
        }
    }
}
