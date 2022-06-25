using WibboEmulator.Communication.Packets.Outgoing.Groups;
using WibboEmulator.Communication.Packets.Outgoing.Users;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Groups;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class SetGroupFavouriteEvent : IPacketEvent
    {
        public double Delay => 100;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null)
            {
                return;
            }

            int GroupId = Packet.PopInt();
            if (GroupId == 0)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            Session.GetUser().FavouriteGroupId = Group.Id;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserStatsDao.UpdateGroupId(dbClient, Session.GetUser().FavouriteGroupId, Session.GetUser().Id);
            }

            if (Session.GetUser().InRoom && Session.GetUser().CurrentRoom != null)
            {
                Session.GetUser().CurrentRoom.SendPacket(new RefreshFavouriteGroupComposer(Session.GetUser().Id));
                if (Group != null)
                {
                    Session.GetUser().CurrentRoom.SendPacket(new UserGroupBadgesComposer(Group));

                    RoomUser User = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                    if (User != null)
                    {
                        Session.GetUser().CurrentRoom.SendPacket(new UpdateFavouriteGroupComposer(Group, User.VirtualId));
                    }
                }
            }
            else
            {
                Session.SendPacket(new RefreshFavouriteGroupComposer(Session.GetUser().Id));
            }
        }
    }
}