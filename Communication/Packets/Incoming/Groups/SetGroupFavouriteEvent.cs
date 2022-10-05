namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Groups;

internal class SetGroupFavouriteEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null)
        {
            return;
        }

        var GroupId = Packet.PopInt();
        if (GroupId == 0)
        {
            return;
        }

        if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out var Group))
        {
            return;
        }

        session.GetUser().FavouriteGroupId = Group.Id;
        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserStatsDao.UpdateGroupId(dbClient, session.GetUser().FavouriteGroupId, session.GetUser().Id);
        }

        if (session.GetUser().InRoom && session.GetUser().CurrentRoom != null)
        {
            session.GetUser().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
            if (Group != null)
            {
                session.GetUser().CurrentRoom.SendPacket(new UserGroupBadgesComposer(Group));

                var User = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
                if (User != null)
                {
                    session.GetUser().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(Group, User.VirtualId));
                }
            }
        }
        else
        {
            session.SendPacket(new RefreshFavouriteGroupComposer(session.GetUser().Id));
        }
    }
}