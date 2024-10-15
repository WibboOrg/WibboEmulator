namespace WibboEmulator.Communication.Packets.Incoming.Messenger;
using WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Users.Messenger;

internal sealed class UserSearchEvent : IPacketEvent
{
    public double Delay => 500;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session.User.Messenger == null)
        {
            return;
        }

        var searchPseudo = packet.PopString(16);
        if (searchPseudo.Length is < 1 or > 16)
        {
            return;
        }

        var searchResult = SearchResultFactory.GetSearchResult(searchPseudo);
        var friend = new List<SearchResult>();
        var other = new List<SearchResult>();

        foreach (var searchResult2 in searchResult)
        {
            if (searchResult2.UserId != Session.User.Id)
            {
                if (Session.User.Messenger.FriendshipExists(searchResult2.UserId))
                {
                    friend.Add(searchResult2);
                }
                else
                {
                    other.Add(searchResult2);
                }
            }
        }

        Session.SendPacket(new UserSearchResultComposer(friend, other));
    }
}
