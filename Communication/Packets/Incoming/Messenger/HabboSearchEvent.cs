using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Game.Clients;
using Butterfly.Game.Users.Messenger;
using Butterfly.Utilities;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class HabboSearchEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo().GetMessenger() == null)
            {
                return;
            }

            string SearchPseudo = StringCharFilter.Escape(Packet.PopString());
            if (SearchPseudo.Length < 1 || SearchPseudo.Length > 100)
            {
                return;
            }

            List<SearchResult> searchResult = SearchResultFactory.GetSearchResult(SearchPseudo);
            List<SearchResult> friend = new List<SearchResult>();
            List<SearchResult> other = new List<SearchResult>();

            foreach (SearchResult searchResult2 in searchResult)
            {
                if (searchResult2.UserId != Session.GetHabbo().Id)
                {
                    if (Session.GetHabbo().GetMessenger().FriendshipExists(searchResult2.UserId))
                    {
                        friend.Add(searchResult2);
                    }
                    else
                    {
                        other.Add(searchResult2);
                    }
                }
            }

            Session.SendPacket(new HabboSearchResultComposer(friend, other));
        }
    }
}
