namespace WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Users;

internal sealed class ProfileInformationComposer : ServerPacket
{
    public ProfileInformationComposer(User habbo, GameClient session, List<Group> groups, int friendCount)
        : base(ServerPacketHeader.USER_PROFILE)
    {
        var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(habbo.AccountCreated);

        this.WriteInteger(habbo.Id);
        this.WriteString(habbo.Username);
        this.WriteString(habbo.Look);
        this.WriteString(habbo.Motto);
        this.WriteString(origin.ToString("dd/MM/yyyy"));
        this.WriteInteger(habbo.AchievementPoints);
        this.WriteInteger(friendCount); // Friend Count
        this.WriteBoolean(habbo.Id != session.User.Id && session.User.Messenger.FriendshipExists(habbo.Id)); //  Is friend
        this.WriteBoolean(habbo.Id != session.User.Id && !session.User.Messenger.FriendshipExists(habbo.Id) && session.User.Messenger.RequestExists(habbo.Id)); // Sent friend request
        this.WriteBoolean(WibboEnvironment.GetGame().GetGameClientManager().GetClientByUserID(habbo.Id) != null);

        this.WriteInteger(groups.Count);
        foreach (var group in groups)
        {
            this.WriteInteger(group.Id);
            this.WriteString(group.Name);
            this.WriteString(group.Badge);
            this.WriteString(WibboEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour1, true));
            this.WriteString(WibboEnvironment.GetGame().GetGroupManager().GetColourCode(group.Colour2, false));
            this.WriteBoolean(habbo.FavouriteGroupId == group.Id); // todo favs
            this.WriteInteger(0);//what the fuck
            this.WriteBoolean(group == null || group.ForumEnabled);//HabboTalk
        }

        this.WriteInteger(Convert.ToInt32(WibboEnvironment.GetUnixTimestamp() - habbo.LastOnline)); // Last online
        this.WriteBoolean(true); // Show the profile
    }
}
