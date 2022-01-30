using Butterfly.Game.Users.Messenger;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Messenger
{
    internal class HabboSearchResultComposer : ServerPacket
    {
        public HabboSearchResultComposer(List<SearchResult> friend, List<SearchResult> other)
            : base(ServerPacketHeader.MESSENGER_SEARCH)
        {
            this.WriteInteger(friend.Count);
            foreach (SearchResult search in friend)
            {
                this.WriteInteger(search.UserId);
                this.WriteString(search.Username);
                this.WriteString(""); //motto
                this.WriteBoolean(ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(search.UserId) != null);
                this.WriteBoolean(false);
                this.WriteString(string.Empty);
                this.WriteInteger(0);
                this.WriteString(search.Look);
                this.WriteString(""); //realName
            }

            this.WriteInteger(other.Count);
            foreach (SearchResult search in other)
            {
                this.WriteInteger(search.UserId);
                this.WriteString(search.Username);
                this.WriteString(""); //motto
                this.WriteBoolean(ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(search.UserId) != null);
                this.WriteBoolean(false);
                this.WriteString(string.Empty);
                this.WriteInteger(0);
                this.WriteString(search.Look);
                this.WriteString(""); //realName
            }
        }
    }
}
