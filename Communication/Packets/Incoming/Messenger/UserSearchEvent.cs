namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Messenger;
using WibboEmulator.Utilities;

internal class UserSearchEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session.User.Messenger == null)
        {
            return;
        }

        var searchPseudo = StringCharFilter.Escape(packet.PopString());
        if (searchPseudo.Length is < 1 or > 100)
        {
            return;
        }

        var searchResult = SearchResultFactory.GetSearchResult(searchPseudo);
        var friend = new List<SearchResult>();
        var other = new List<SearchResult>();

        foreach (var searchResult2 in searchResult)
        {
            if (searchResult2.UserId != session.User.Id)
            {
                if (session.User.Messenger.FriendshipExists(searchResult2.UserId))
                {
                    friend.Add(searchResult2);
                }
                else
                {
                    other.Add(searchResult2);
                }
            }
        }

        session.SendPacket(new UserSearchResultComposer(friend, other));
    }
}
