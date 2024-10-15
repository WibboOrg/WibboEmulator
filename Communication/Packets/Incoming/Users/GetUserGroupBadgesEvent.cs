namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;

internal sealed class GetUserGroupBadgesEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null || Session.User.LoadingRoomId == 0)
        {
            return;
        }

        if (!RoomManager.TryGetRoom(Session.User.LoadingRoomId, out var room))
        {
            return;
        }

        var badges = new Dictionary<int, string>();
        foreach (var user in room.RoomUserManager.RoomUsers.ToList())
        {
            if (user.IsBot || user.Client == null || user.Client.User == null)
            {
                continue;
            }

            if (user.Client.User.FavouriteGroupId == 0 || badges.ContainsKey(user.Client.User.FavouriteGroupId))
            {
                continue;
            }

            if (!GroupManager.TryGetGroup(user.Client.User.FavouriteGroupId, out var group))
            {
                continue;
            }

            if (!badges.ContainsKey(group.Id))
            {
                badges.Add(group.Id, group.Badge);
            }
        }

        if (Session.User.FavouriteGroupId > 0)
        {
            if (GroupManager.TryGetGroup(Session.User.FavouriteGroupId, out var group))
            {
                if (!badges.ContainsKey(group.Id))
                {
                    badges.Add(group.Id, group.Badge);
                }
            }
        }

        room.SendPacket(new UserGroupBadgesComposer(badges));
        Session.SendPacket(new UserGroupBadgesComposer(badges));
    }
}
