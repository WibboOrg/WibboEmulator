using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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