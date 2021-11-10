using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Rooms;

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
                UserFavoriteDao.Delete(dbClient, Session.GetHabbo().Id, RoomId);
            }
        }
    }
}
