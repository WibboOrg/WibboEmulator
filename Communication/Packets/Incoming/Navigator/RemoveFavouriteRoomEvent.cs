using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RemoveFavouriteRoomEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            int roomId = Packet.PopInt();

            RoomData roomdata = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(roomId);
            if (roomdata == null)
            {
                return;
            }

            Session.GetHabbo().FavoriteRooms.Remove(roomId);

            Session.SendPacket(new UpdateFavouriteRoomComposer(roomId, false));

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserFavoriteDao.Delete(dbClient, Session.GetHabbo().Id, roomId);
            }
        }
    }
}
