using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class RemoveRightsEvent : IPacketEvent
    {
        public double Delay => 250;

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

                RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(UserId);
                if (roomUserByUserId != null && !roomUserByUserId.IsBot)
                {
                    roomUserByUserId.GetClient().SendPacket(new YouAreControllerComposer(0));

                    roomUserByUserId.RemoveStatus("flatctrl");
                    roomUserByUserId.SetStatus("flatctrl", "0");
                    roomUserByUserId.UpdateNeeded = true;
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

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            RoomRightDao.DeleteList(dbClient, room.Id, userIds);
        }
    }
}
