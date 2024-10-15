namespace WibboEmulator.Communication.Packets.Outgoing.Navigator.New;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Navigators;

internal sealed class NavigatorSearchResultSetComposer : ServerPacket
{
    public NavigatorSearchResultSetComposer(string category, string data, ICollection<SearchResultList> searchResultLists, GameClient Session, int goBack = 1, int fetchLimit = 12)
        : base(ServerPacketHeader.NAVIGATOR_SEARCH)
    {
        this.WriteString(category);//Search code.
        this.WriteString(data);//Text?

        this.WriteInteger(searchResultLists.Count);//Count
        foreach (var searchResult in searchResultLists.ToList())
        {
            this.WriteString(searchResult.CategoryIdentifier);
            this.WriteString(searchResult.PublicName);
            this.WriteInteger(searchResult.SearchAllowance != 0 ? goBack : (int)searchResult.SearchAllowance);//0 = nothing, 1 = show more, 2 = back Action allowed.
            this.WriteBoolean(searchResult.Minimized);//True = minimized, false = open.
            this.WriteInteger(searchResult.ViewMode == NavigatorViewMode.Regular ? 0 : searchResult.ViewMode == NavigatorViewMode.Thumbnail ? 1 : 0);//View mode, 0 = tiny/regular, 1 = thumbnail

            NavigatorHandler.Search(this, searchResult, data, Session, fetchLimit);
        }
    }
}
