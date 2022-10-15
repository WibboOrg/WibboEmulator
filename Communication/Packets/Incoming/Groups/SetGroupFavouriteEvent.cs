namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

internal class SetGroupFavouriteEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null)
        {
            return;
        }

        var groupId = packet.PopInt();
        if (groupId == 0)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(groupId, out var group))
        {
            return;
        }

        session.
        User.FavouriteGroupId = group.Id;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserStatsDao.UpdateGroupId(dbClient, session.User.FavouriteGroupId, session.User.Id);
        }

        if (session.User.InRoom && session.User.CurrentRoom != null)
        {
            session.User.CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.User.Id));
            if (group != null)
            {
                session.User.CurrentRoom.SendPacket(new UserGroupBadgesComposer(group));

                var user = session.User.CurrentRoom.RoomUserManager.GetRoomUserByUserId(session.User.Id);
                if (user != null)
                {
                    session.User.CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(group, user.VirtualId));
                }
            }
        }
        else
        {
            session.SendPacket(new RefreshFavouriteGroupComposer(session.User.Id));
        }
    }
}
