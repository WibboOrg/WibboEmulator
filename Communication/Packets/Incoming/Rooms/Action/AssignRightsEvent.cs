using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using Butterfly.Game.Users;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class AssignRightsEvent : IPacketEvent
    {
        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            int UserId = Packet.PopInt();

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null)
            {
                return;
            }

            if (room == null || !room.CheckRights(Session, true))
            {
                return;
            }

            if (room.UsersWithRights.Contains(UserId))
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("user.giverights.error", Session.Langue));
            }
            else
            {
                User Userright = ButterflyEnvironment.GetHabboById(UserId);
                if (Userright == null)
                {
                    return;
                }

                room.UsersWithRights.Add(UserId);

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    RoomRightDao.Insert(dbClient, room.Id, UserId);
                }

                ServerPacket Response = new ServerPacket(ServerPacketHeader.ROOM_RIGHTS_LIST_ADD);
                Response.WriteInteger(room.Id);
                Response.WriteInteger(UserId);
                Response.WriteString(Userright.Username);
                Session.SendPacket(Response);

                RoomUser roomUserByHabbo = room.GetRoomUserManager().GetRoomUserByHabboId(UserId);
                if (roomUserByHabbo == null || roomUserByHabbo.IsBot)
                {
                    return;
                }

                roomUserByHabbo.RemoveStatus("flatctrl 0");
                roomUserByHabbo.SetStatus("flatctrl 1", "");
                roomUserByHabbo.UpdateNeeded = true;

                roomUserByHabbo.GetClient().SendPacket(new YouAreControllerComposer(1));
            }
        }
    }
}