namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator.New;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Navigators;

internal sealed class NavigatorSearchEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var category = packet.PopString();
        var search = packet.PopString();

        ICollection<SearchResultList> categories = [];

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
                    Session.SendPacket(new NavigatorSearchResultSetComposer(category, search, categories, Session, 2, 50));
                    return;
                }
            }
        }

        Session.SendPacket(new NavigatorSearchResultSetComposer(category, search, categories, Session));
    }
}
