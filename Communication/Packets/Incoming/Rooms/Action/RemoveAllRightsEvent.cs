using Wibbo.Communication.Packets.Outgoing.Rooms.Permissions;
using Wibbo.Communication.Packets.Outgoing.Rooms.Settings;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class RemoveAllRightsEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || !room.CheckRights(Session, true))
            {
                return;
            }

            if (room.UsersWithRights.Count == 0)
            {
                return;
            }

            foreach (int num in room.UsersWithRights)
            {
                RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(num);
                if (roomUserByUserId != null)
                {
                    if (!roomUserByUserId.IsBot)
                    {
                        roomUserByUserId.GetClient().SendPacket(new YouAreControllerComposer(0));

                        roomUserByUserId.UpdateNeeded = true;
                    }
                }

                Session.SendPacket(new FlatControllerRemovedMessageComposer(room.Id, num));
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomRightDao.Delete(dbClient, room.Id);
            }

            room.UsersWithRights.Clear();

            Session.SendPacket(new RoomRightsListComposer(room));
        }
    }
}
