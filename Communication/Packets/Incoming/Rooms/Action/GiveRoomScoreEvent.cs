using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
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

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetUser().CurrentRoomId);
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

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateScore(dbClient, room.Id, room.RoomData.Score);
            }

            Session.GetUser().RatedRooms.Add(room.Id);
            Session.SendPacket(new RoomRatingComposer(room.RoomData.Score, !(Session.GetUser().RatedRooms.Contains(room.Id) || room.RoomData.OwnerId == Session.GetUser().Id)));
        }
    }
}
