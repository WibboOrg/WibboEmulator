using Butterfly.Communication.Packets.Outgoing;
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

            int num = Packet.PopInt();
            RoomData roomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(num);
            if (roomData == null || Session.GetHabbo().FavoriteRooms.Count >= 30 || (Session.GetHabbo().FavoriteRooms.Contains(roomData)))
            {
                ServerPacket Response = new ServerPacket(33);
                Response.WriteInteger(-9001);
            }
            else
            {
                ServerPacket Response = new ServerPacket(ServerPacketHeader.USER_FAVORITE_ROOM);
                Response.WriteInteger(num);
                Response.WriteBoolean(true);
                Session.SendPacket(Response);
                Session.GetHabbo().FavoriteRooms.Add(roomData);
                using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                {
                    queryreactor.RunQuery("INSERT INTO user_favorites (user_id,room_id) VALUES (" + Session.GetHabbo().Id + "," + num + ")");
                }
            }
        }
    }
}