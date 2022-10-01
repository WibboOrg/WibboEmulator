using WibboEmulator.Communication.Packets.Outgoing.Rooms.Permissions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Settings;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class AssignRightsEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            int UserId = Packet.PopInt();

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (!room.CheckRights(Session, true))
            {
                return;
            }

            if (room.UsersWithRights.Contains(UserId))
            {
                Session.SendNotification(WibboEnvironment.GetLanguageManager().TryGetValue("user.giverights.error", Session.Langue));
            }
            else
            {
                User Userright = WibboEnvironment.GetUserById(UserId);
                if (Userright == null)
                {
                    return;
                }

                room.UsersWithRights.Add(UserId);

                using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    RoomRightDao.Insert(dbClient, room.Id, UserId);
                }

                Session.SendPacket(new FlatControllerAddedComposer(room.Id, UserId, Userright.Username));

                RoomUser roomUserByUserId = room.GetRoomUserManager().GetRoomUserByUserId(UserId);
                if (roomUserByUserId == null || roomUserByUserId.IsBot)
                {
                    return;
                }

                roomUserByUserId.RemoveStatus("flatctrl");
                roomUserByUserId.SetStatus("flatctrl", "1");
                roomUserByUserId.UpdateNeeded = true;

                roomUserByUserId.GetClient().SendPacket(new YouAreControllerComposer(1));
            }
        }
    }
}