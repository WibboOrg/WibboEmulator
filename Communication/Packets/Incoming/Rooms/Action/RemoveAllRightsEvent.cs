using Butterfly.Communication.Packets.Outgoing;
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
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
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
                RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(num);
                if (roomUserByHabbo != null)
                {
                    if (!roomUserByHabbo.IsBot)
                    {
                        roomUserByHabbo.GetClient().SendPacket(new YouAreControllerComposer(0));

                        roomUserByHabbo.UpdateNeeded = true;
                    }
                }

                Session.SendPacket(new FlatControllerRemovedMessageComposer(room.Id, num));
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomRightDao.Delete(dbClient, room.Id);
            }

            room.UsersWithRights.Clear();

            ServerPacket Response2 = new ServerPacket(ServerPacketHeader.ROOM_RIGHTS_LIST);
            Response2.WriteInteger(room.RoomData.Id);
            Response2.WriteInteger(0);
            Session.SendPacket(Response2);
        }
    }
}
