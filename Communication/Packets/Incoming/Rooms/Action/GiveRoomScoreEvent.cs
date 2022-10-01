using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class GiveRoomScoreEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session.GetUser() == null)
            {
                return;
            }

            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room room))
                return;

            if (Session.GetUser().RatedRooms.Contains(room.Id) || room.CheckRights(Session, true))
            {
                return;
            }

            int score = Packet.PopInt();
            switch (score)
            {
                case -1:
                    room.RoomData.Score--;
                    break;
                case 0:
                    return;
                case 1:
                    room.RoomData.Score++;
                    break;
                default:
                    return;
            }

            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateScore(dbClient, room.Id, room.RoomData.Score);
            }

            Session.GetUser().RatedRooms.Add(room.Id);
            Session.SendPacket(new RoomRatingComposer(room.RoomData.Score, !(Session.GetUser().RatedRooms.Contains(room.Id) || room.RoomData.OwnerId == Session.GetUser().Id)));
        }
    }
}
