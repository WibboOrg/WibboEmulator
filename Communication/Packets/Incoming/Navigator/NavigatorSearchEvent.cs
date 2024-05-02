namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator.New;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Navigators;

internal sealed class NavigatorSearchEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var category = packet.PopString();
        var search = packet.PopString();

        ICollection<SearchResultList> categories = new List<SearchResultList>();

        if (!string.IsNullOrEmpty(search))
        {
            if (NavigatorManager.TryGetSearchResultList(0, out var queryResult))
            {
                categories.Add(queryResult);
            }
        }
        else
        {
            categories = NavigatorManager.GetCategorysForSearch(category);
            if (categories.Count == 0)
            {
                //Are we going in deep?!
                categories = NavigatorManager.GetResultByIdentifier(category);
                if (categories.Count > 0)
                {
                    session.SendPacket(new NavigatorSearchResultSetComposer(category, search, categories, session, 2, 50));
                    return;
                }
            }
        }

        session.SendPacket(new NavigatorSearchResultSetComposer(category, search, categories, session));
    }
}
