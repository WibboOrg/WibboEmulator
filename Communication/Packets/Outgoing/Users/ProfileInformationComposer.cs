using Butterfly.Game.GameClients;
using Butterfly.Game.Guilds;
using Butterfly.Game.User;
using System;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class ProfileInformationComposer : ServerPacket
    {
        public ProfileInformationComposer(Habbo habbo, GameClient session, List<Guild> groups, int friendCount)
            : base(ServerPacketHeader.USER_PROFILE)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(habbo.AccountCreated);

            this.WriteInteger(habbo.Id);
            this.WriteString(habbo.Username);
            this.WriteString(habbo.Look);
            this.WriteString(habbo.Motto);
            this.WriteString(origin.ToString("dd/MM/yyyy"));
            this.WriteInteger(habbo.AchievementPoints);
            this.WriteInteger(friendCount); // Friend Count
            this.WriteBoolean(habbo.Id != session.GetHabbo().Id && session.GetHabbo().GetMessenger().FriendshipExists(habbo.Id)); //  Is friend
            this.WriteBoolean(habbo.Id != session.GetHabbo().Id && !session.GetHabbo().GetMessenger().FriendshipExists(habbo.Id) && session.GetHabbo().GetMessenger().RequestExists(habbo.Id)); // Sent friend request
            this.WriteBoolean((ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(habbo.Id)) != null);

            this.WriteInteger(groups.Count);
            foreach (Guild group in groups)
            {
                this.WriteInteger(group.Id);
                this.WriteString(group.Name);
                this.WriteString(group.Badge);
                this.WriteString(ButterflyEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                this.WriteString(ButterflyEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                this.WriteBoolean(habbo.FavouriteGroupId == group.Id); // todo favs
                this.WriteInteger(0);//what the fuck
                this.WriteBoolean(group != null ? group.ForumEnabled : true);//HabboTalk
            }

            this.WriteInteger(Convert.ToInt32(ButterflyEnvironment.GetUnixTimestamp() - habbo.LastOnline)); // Last online
            this.WriteBoolean(true); // Show the profile
        }
    }
}
