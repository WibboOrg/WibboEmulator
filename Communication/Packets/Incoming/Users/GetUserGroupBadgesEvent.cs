namespace WibboEmulator.Communication.Packets.Incoming.Users;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Games.GameClients;

internal class GetUserGroupBadgesEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.GetUser() == null || session.GetUser().LoadingRoomId == 0)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().LoadingRoomId, out var room))
        {
            return;
        }

        var badges = new Dictionary<int, string>();
        foreach (var user in room.GetRoomUserManager().GetRoomUsers().ToList())
        {
            if (user.IsBot || user.Client == null || user.Client.GetUser() == null)
            {
                continue;
            }

            if (user.Client.GetUser().FavouriteGroupId == 0 || badges.ContainsKey(user.Client.GetUser().FavouriteGroupId))
            {
                continue;
            }

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(user.Client.GetUser().FavouriteGroupId, out var group))
            {
                continue;
            }

            if (!badges.ContainsKey(group.Id))
            {
                badges.Add(group.Id, group.Badge);
            }
        }

        if (session.GetUser().FavouriteGroupId > 0)
        {
            if (WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(session.GetUser().FavouriteGroupId, out var group))
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
