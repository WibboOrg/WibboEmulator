using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class AddFavouriteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            int roomId = Packet.PopInt();

            RoomData roomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
            if (roomData == null || Session.GetHabbo().FavoriteRooms.Count >= 30 || (Session.GetHabbo().FavoriteRooms.Contains(roomData)))
            {
                return;
            }
            else
            {
                ServerPacket Response = new ServerPacket(ServerPacketHeader.USER_FAVORITE_ROOM);
                Response.WriteInteger(roomId);
                Response.WriteBoolean(true);
                Session.SendPacket(Response);

                Session.GetHabbo().FavoriteRooms.Add(roomData);

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    UserFavoriteDao.Insert(dbClient, Session.GetHabbo().Id, roomId);
            }
        }
    }
}