using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class AddFavouriteRoomEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            int roomId = Packet.PopInt();

            RoomData roomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
            if (roomData == null || Session.GetHabbo().FavoriteRooms.Count >= 30 || (Session.GetHabbo().FavoriteRooms.Contains(roomId)))
            {
                return;
            }
            else
            {
                Session.SendPacket(new UpdateFavouriteRoomComposer(roomId, true));

                Session.GetHabbo().FavoriteRooms.Add(roomId);

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    UserFavoriteDao.Insert(dbClient, Session.GetHabbo().Id, roomId);
            }
        }
    }
}