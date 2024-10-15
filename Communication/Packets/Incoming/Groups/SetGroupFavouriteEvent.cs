namespace WibboEmulator.Communication.Packets.Incoming.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal sealed class SetGroupFavouriteEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null)
        {
            return;
        }

        var groupId = packet.PopInt();
        if (groupId == 0)
        {
            return;
        }

        if (!GroupManager.TryGetGroup(groupId, out var group))
        {
            return;
        }

        Session.User.FavouriteGroupId = group.Id;
        using (var dbClient = DatabaseManager.Connection)
        {
            UserStatsDao.UpdateGroupId(dbClient, Session.User.FavouriteGroupId, Session.User.Id);
        }

        if (Session.User.InRoom && Session.User.Room != null)
        {
            Session.User.Room.SendPacket(new RefreshFavouriteGroupComposer(Session.User.Id));
            if (group != null)
            {
                Session.User.Room.SendPacket(new UserGroupBadgesComposer(group));

                var user = Session.User.Room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
                if (user != null)
                {
                    Session.User.Room.SendPacket(new UpdateFavouriteGroupComposer(group, user.VirtualId));
                }
            }
        }
        else
        {
            Session.SendPacket(new RefreshFavouriteGroupComposer(Session.User.Id));
        }
    }
}
