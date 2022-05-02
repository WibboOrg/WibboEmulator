using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Groups;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
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

            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(GroupId, out Group Group))
            {
                return;
            }

            Session.GetUser().FavouriteGroupId = Group.Id;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
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