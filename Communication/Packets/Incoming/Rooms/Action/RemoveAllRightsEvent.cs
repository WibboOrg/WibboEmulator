using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
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

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomRightDao.Delete(dbClient, room.Id);
            }

            room.UsersWithRights.Clear();

            Session.SendPacket(new RoomRightsListComposer(room));
        }
    }
}
