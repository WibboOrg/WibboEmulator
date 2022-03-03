using Butterfly.Game.Clients;
using Butterfly.Game.Guilds;
using Butterfly.Game.Users;
using System;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Users
{
    internal class ProfileInformationComposer : ServerPacket
    {
        public ProfileInformationComposer(User user, Client session, List<Group> groups, int friendCount)
            : base(ServerPacketHeader.USER_PROFILE)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(user.AccountCreated);

            this.WriteInteger(user.Id);
            this.WriteString(user.Username);
            this.WriteString(user.Look);
            this.WriteString(user.Motto);
            this.WriteString(origin.ToString("dd/MM/yyyy"));
            this.WriteInteger(user.AchievementPoints);
            this.WriteInteger(friendCount); // Friend Count
            this.WriteBoolean(user.Id != session.GetUser().Id && session.GetUser().GetMessenger().FriendshipExists(user.Id)); //  Is friend
            this.WriteBoolean(user.Id != session.GetUser().Id && !session.GetUser().GetMessenger().FriendshipExists(user.Id) && session.GetUser().GetMessenger().RequestExists(user.Id)); // Sent friend request
            this.WriteBoolean((ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(user.Id)) != null);

            this.WriteInteger(groups.Count);
            foreach (Group group in groups)
            {
                this.WriteInteger(group.Id);
                this.WriteString(group.Name);
                this.WriteString(group.Badge);
                this.WriteString(ButterflyEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                this.WriteString(ButterflyEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                this.WriteBoolean(user.FavouriteGroupId == group.Id); // todo favs
                this.WriteInteger(0);//what the fuck
                this.WriteBoolean(group != null ? group.ForumEnabled : true);
            }

            this.WriteInteger(Convert.ToInt32(ButterflyEnvironment.GetUnixTimestamp() - user.LastOnline)); // Last online
            this.WriteBoolean(true); // Show the profile
        }
    }
}
