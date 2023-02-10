namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;

internal sealed class GetUserGroupBadgesEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.User == null || session.User.LoadingRoomId == 0)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.LoadingRoomId, out var room))
        {
            return;
        }

        var badges = new Dictionary<int, string>();
        foreach (var user in room.RoomUserManager.GetRoomUsers().ToList())
        {
            if (user.IsBot || user.Client == null || user.Client.User == null)
            {
                continue;
            }

            if (user.Client.User.FavouriteGroupId == 0 || badges.ContainsKey(user.Client.User.FavouriteGroupId))
            {
                continue;
            }

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(user.Client.User.FavouriteGroupId, out var group))
            {
                continue;
            }

            if (!badges.ContainsKey(group.Id))
            {
                badges.Add(group.Id, group.Badge);
            }
        }

        if (session.User.FavouriteGroupId > 0)
        {
            if (WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(session.User.FavouriteGroupId, out var group))
            {
                if (!badges.ContainsKey(group.Id))
                {
                    badges.Add(group.Id, group.Badge);
                }
            }
        }

        room.SendPacket(new UserGroupBadgesComposer(badges));
        session.SendPacket(new UserGroupBadgesComposer(badges));
    }
}
