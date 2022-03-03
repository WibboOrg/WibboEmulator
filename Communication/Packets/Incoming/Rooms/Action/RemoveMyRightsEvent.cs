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
            if (Session.GetUser() == null)
            {
                return;
            }

            if (!Session.GetUser().InRoom)
            {
                return;
            }

            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room Room))
            {
                return;
            }

            if (!Room.CheckRights(Session))
            {
                return;
            }

            if (Room.UsersWithRights.Contains(Session.GetUser().Id))
            {
                RoomUser User = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
                if (User != null && !User.IsBot)
                {
                    User.RemoveStatus("flatctrl 1");
                    User.UpdateNeeded = true;

                    User.GetClient().SendPacket(new YouAreNotControllerComposer());
                }

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    RoomRightDao.Delete(dbClient, Room.Id, Session.GetUser().Id);
                }

                if (Room.UsersWithRights.Contains(Session.GetUser().Id))
                {
                    Room.UsersWithRights.Remove(Session.GetUser().Id);
                }
            }
        }
    }
}
