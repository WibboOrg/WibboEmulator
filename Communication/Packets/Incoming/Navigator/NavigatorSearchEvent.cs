using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Navigators;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class NavigatorSearchEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            string Category = Packet.PopString();
            string Search = Packet.PopString();

            ICollection<SearchResultList> Categories = new List<SearchResultList>();

            if (!string.IsNullOrEmpty(Search))
            {
                if (ButterflyEnvironment.GetGame().GetNavigator().TryGetSearchResultList(0, out SearchResultList QueryResult))
                {
                    Categories.Add(QueryResult);
                }
            }
            else
            {
                Categories = ButterflyEnvironment.GetGame().GetNavigator().GetCategorysForSearch(Category);
                if (Categories.Count == 0)
                {
                    //Are we going in deep?!
                    Categories = ButterflyEnvironment.GetGame().GetNavigator().GetResultByIdentifier(Category);
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
