namespace WibboEmulator.Communication.Packets.Outgoing.Messenger;
using WibboEmulator.Games.Users.Messenger;

internal class UserSearchResultComposer : ServerPacket
{
    public UserSearchResultComposer(List<SearchResult> friend, List<SearchResult> other)
        : base(ServerPacketHeader.MESSENGER_SEARCH)
    {
        this.WriteInteger(friend.Count);
        foreach (var search in friend)
        {
            this.WriteInteger(search.UserId);
            this.WriteString(search.Username);
            this.WriteString(""); //motto
            this.WriteBoolean(WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(search.UserId) != null);
            this.WriteBoolean(false);
            this.WriteString(string.Empty);
            this.WriteInteger(0);
            this.WriteString(search.Look);
            this.WriteString(""); //realName
        }

        this.WriteInteger(other.Count);
        foreach (var search in other)
        {
            this.WriteInteger(search.UserId);
            this.WriteString(search.Username);
            this.WriteString(""); //motto
            this.WriteBoolean(WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(search.UserId) != null);
            this.WriteBoolean(false);
            this.WriteString(string.Empty);
            this.WriteInteger(0);
            this.WriteString(search.Look);
            this.WriteString(""); //realName
        }
    }
}
