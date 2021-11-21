using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.User;
using System.Collections.Generic;
using System.Text;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RemoveRightsEvent : IPacketEvent
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

            List<int> userIds = new List<int>();
            int Amount = Packet.PopInt();
            for (int index = 0; index < Amount; ++index)
            {
                int UserId = Packet.PopInt();
                if (room.UsersWithRights.Contains(UserId))
                {
                    room.UsersWithRights.Remove(UserId);
                }

                if(!userIds.Contains(UserId))
                    userIds.Add(UserId);

                RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(UserId);
                if (roomUserByHabbo != null && !roomUserByHabbo.IsBot)
                {
                    roomUserByHabbo.GetClient().SendPacket(new YouAreControllerComposer(0));

                    roomUserByHabbo.RemoveStatus("flatctrl 1");
                    roomUserByHabbo.SetStatus("flatctrl 0", "");
                    roomUserByHabbo.UpdateNeeded = true;
                }
                ServerPacket Response2 = new ServerPacket(ServerPacketHeader.ROOM_RIGHTS_LIST_REMOVE);
                Response2.WriteInteger(room.Id);
                Response2.WriteInteger(UserId);
                Session.SendPacket(Response2);

                if (room.UsersWithRights.Count <= 0)
                {
                    ServerPacket Response3 = new ServerPacket(ServerPacketHeader.ROOM_RIGHTS_LIST);
                    Response3.WriteInteger(room.RoomData.Id);
                    Response3.WriteInteger(0);
                    Session.SendPacket(Response3);
                }
                else
                {

                    ServerPacket Response = new ServerPacket(ServerPacketHeader.ROOM_RIGHTS_LIST);
                    Response.WriteInteger(room.RoomData.Id);
                    Response.WriteInteger(room.UsersWithRights.Count);
                    foreach (int UserId2 in room.UsersWithRights)
                    {
                        Habbo habboForId = ButterflyEnvironment.GetHabboById(UserId2);
                        Response.WriteInteger(UserId2);
                        Response.WriteString((habboForId == null) ? "Undefined (error)" : habboForId.Username);
                    }
                    Session.SendPacket(Response);
                }
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomRightDao.DeleteList(dbClient, room.Id, userIds);
            }
        }
    }
}
