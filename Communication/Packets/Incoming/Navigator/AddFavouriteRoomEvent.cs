using Wibbo.Communication.Packets.Outgoing.Navigator;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class AddFavouriteRoomEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            int roomId = Packet.PopInt();

            RoomData roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
            if (roomData == null || Session.GetUser().FavoriteRooms.Count >= 30 || (Session.GetUser().FavoriteRooms.Contains(roomId)))
            {
                return;
            }
            else
            {
                Session.SendPacket(new UpdateFavouriteRoomComposer(roomId, true));

                Session.GetUser().FavoriteRooms.Add(roomId);

                using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                UserFavoriteDao.Insert(dbClient, Session.GetUser().Id, roomId);
            }
        }
    }
}