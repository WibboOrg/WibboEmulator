using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Users;

namespace WibboEmulator.Communication.Packets.Outgoing.Users
{
    internal class ProfileInformationComposer : ServerPacket
    {
        public ProfileInformationComposer(User habbo, Client session, List<Group> groups, int friendCount)
            : base(ServerPacketHeader.USER_PROFILE)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(habbo.AccountCreated);

            WriteInteger(habbo.Id);
            WriteString(habbo.Username);
            WriteString(habbo.Look);
            WriteString(habbo.Motto);
            WriteString(origin.ToString("dd/MM/yyyy"));
            WriteInteger(habbo.AchievementPoints);
            WriteInteger(friendCount); // Friend Count
            this.WriteBoolean(habbo.Id != session.GetUser().Id && session.GetUser().GetMessenger().FriendshipExists(habbo.Id)); //  Is friend
            this.WriteBoolean(habbo.Id != session.GetUser().Id && !session.GetUser().GetMessenger().FriendshipExists(habbo.Id) && session.GetUser().GetMessenger().RequestExists(habbo.Id)); // Sent friend request
            WriteBoolean((WibboEnvironment.GetGame().GetClientManager().GetClientByUserID(habbo.Id)) != null);

            WriteInteger(groups.Count);
            foreach (Group group in groups)
            {
                WriteInteger(group.Id);
                WriteString(group.Name);
                WriteString(group.Badge);
                WriteString(WibboEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
                WriteString(WibboEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
                WriteBoolean(habbo.FavouriteGroupId == group.Id); // todo favs
                WriteInteger(0);//what the fuck
                WriteBoolean(group != null ? group.ForumEnabled : true);//HabboTalk
            }

            WriteInteger(Convert.ToInt32(WibboEnvironment.GetUnixTimestamp() - habbo.LastOnline)); // Last online
            WriteBoolean(true); // Show the profile
        }
    }
}
