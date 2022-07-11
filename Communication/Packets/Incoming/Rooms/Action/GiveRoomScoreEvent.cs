using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

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

            Room room = WibboEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
            if (room == null || Session.GetUser().RatedRooms.Contains(room.Id) || room.CheckRights(Session, true))
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
