using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Users;
using System.Collections.Generic;

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

                Session.SendPacket(new FlatControllerRemovedMessageComposer(room.Id, UserId));

                if (room.UsersWithRights.Count <= 0)
                {
                    Session.SendPacket(new RoomRightsListComposer(room));
                }
                else
                {
                    room.UsersWithRights.Contains(UserId);
                    Session.SendPacket(new RoomRightsListComposer(room));
                }
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomRightDao.DeleteList(dbClient, room.Id, userIds);
            }
        }
    }
}
