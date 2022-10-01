using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session, true))
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
