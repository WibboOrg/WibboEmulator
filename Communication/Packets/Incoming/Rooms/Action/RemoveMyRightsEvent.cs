using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room Room))
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
                    User.RemoveStatus("flatctrl");
                    User.UpdateNeeded = true;

                    User.GetClient().SendPacket(new YouAreNotControllerComposer());
                }

                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
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
