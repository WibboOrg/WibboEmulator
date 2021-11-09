using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RemoveAllRightsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
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

                ServerPacket Response3 = new ServerPacket(ServerPacketHeader.ROOM_RIGHTS_LIST_REMOVE);
                Response3.WriteInteger(room.Id);
                Response3.WriteInteger(num);
                Session.SendPacket(Response3);
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
