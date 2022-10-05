namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Navigator.New;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Navigator;

internal class NavigatorSearchEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        var Category = Packet.PopString();
        var Search = Packet.PopString();

        ICollection<SearchResultList> Categories = new List<SearchResultList>();

        if (!string.IsNullOrEmpty(Search))
        {
            if (WibboEnvironment.GetGame().GetNavigator().TryGetSearchResultList(0, out var QueryResult))
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
                    session.SendPacket(new NavigatorSearchResultSetComposer(Category, Search, Categories, session, 2, 50));
                    return;
                }
            }
        }

        session.SendPacket(new NavigatorSearchResultSetComposer(Category, Search, Categories, session));
    }
}
