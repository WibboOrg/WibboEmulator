using Wibbo.Communication.Packets.Outgoing.Navigator;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class RemoveFavouriteRoomEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            int roomId = Packet.PopInt();

            RoomData roomdata = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
            if (roomdata == null)
            {
                return;
            }

            Session.GetUser().FavoriteRooms.Remove(roomId);

            Session.SendPacket(new UpdateFavouriteRoomComposer(roomId, false));

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            UserFavoriteDao.Delete(dbClient, Session.GetUser().Id, roomId);
        }
    }
}
