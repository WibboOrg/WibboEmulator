using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class GiveRoomScoreEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session.GetHabbo() == null)
            {
                return;
            }

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(Session.GetHabbo().CurrentRoomId);
            if (room == null || Session.GetHabbo().RatedRooms.Contains(room.Id) || room.CheckRights(Session, true))
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

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE rooms SET score = '" + room.RoomData.Score + "' WHERE id = '" + room.Id + "';");
            }

            Session.GetHabbo().RatedRooms.Add(room.Id);
            Session.SendPacket(new RoomRatingComposer(room.RoomData.Score, !(Session.GetHabbo().RatedRooms.Contains(room.Id) || room.RoomData.OwnerId == Session.GetHabbo().Id)));
        }
    }
}
