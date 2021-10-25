using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RemoveFavouriteRoomEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            int RoomId = Packet.PopInt();

            RoomData roomdata = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (roomdata == null)
            {
                return;
            }

            Session.GetHabbo().FavoriteRooms.Remove(roomdata);

            ServerPacket Response = new ServerPacket(ServerPacketHeader.USER_FAVORITE_ROOM);
            Response.WriteInteger(RoomId);
            Response.WriteBoolean(false);
            Session.SendPacket(Response);

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.RunQuery("DELETE FROM user_favorites WHERE user_id = '" + Session.GetHabbo().Id + "' AND room_id = '" + RoomId + "'");
            }
        }
    }
}
