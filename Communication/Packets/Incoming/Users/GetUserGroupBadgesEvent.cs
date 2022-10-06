namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal class GetUserGroupBadgesEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null || session.GetUser().LoadingRoomId == 0)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().LoadingRoomId, out var room))
        {
            return;
        }

        var Badges = new Dictionary<int, string>();
        foreach (var User in room.GetRoomUserManager().GetRoomUsers().ToList())
        {
            if (User.IsBot || User.GetClient() == null || User.GetClient().GetUser() == null)
            {
                continue;
            }

            if (User.GetClient().GetUser().FavouriteGroupId == 0 || Badges.ContainsKey(User.GetClient().GetUser().FavouriteGroupId))
            {
                continue;
            }

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(User.GetClient().GetUser().FavouriteGroupId, out var Group))
            {
                continue;
            }

            if (!Badges.ContainsKey(Group.Id))
            {
                Badges.Add(Group.Id, Group.Badge);
            }
        }

        if (session.GetUser().FavouriteGroupId > 0)
        {
            if (WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(session.GetUser().FavouriteGroupId, out var Group))
            {
                if (!Badges.ContainsKey(Group.Id))
                {
                    Badges.Add(Group.Id, Group.Badge);
                }
            }
        }

        room.SendPacket(new UserGroupBadgesComposer(Badges));
        session.SendPacket(new UserGroupBadgesComposer(Badges));
    }
}
