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

        session.GetUser().FavouriteGroupId = group.Id;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserStatsDao.UpdateGroupId(dbClient, session.GetUser().FavouriteGroupId, session.GetUser().Id);
        }

        if (session.GetUser().InRoom && session.GetUser().CurrentRoom != null)
        {
            session.GetUser().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
            if (group != null)
            {
                session.GetUser().CurrentRoom.SendPacket(new UserGroupBadgesComposer(group));

                var user = session.GetUser().CurrentRoom.RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);
                if (user != null)
                {
                    session.GetUser().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(group, user.VirtualId));
                }
            }
        }
        else
        {
            session.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
        }
    }
}
