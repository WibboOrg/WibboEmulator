using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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
