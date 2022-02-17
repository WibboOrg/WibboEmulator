using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RemoveMyRightsEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session))
            {
                return;
            }

            if (Room.UsersWithRights.Contains(Session.GetHabbo().Id))
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
                if (User != null && !User.IsBot)
                {
                    User.RemoveStatus("flatctrl 1");
                    User.UpdateNeeded = true;

                    User.GetClient().SendPacket(new YouAreNotControllerComposer());
                }

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    RoomRightDao.Delete(dbClient, Room.Id, Session.GetHabbo().Id);
                }

                if (Room.UsersWithRights.Contains(Session.GetHabbo().Id))
                {
                    Room.UsersWithRights.Remove(Session.GetHabbo().Id);
                }
            }
        }
    }
}
