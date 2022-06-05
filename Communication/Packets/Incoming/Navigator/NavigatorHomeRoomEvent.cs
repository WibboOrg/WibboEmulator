using Wibbo.Communication.Packets.Outgoing.Navigator;
using Wibbo.Database.Daos;
using Wibbo.Database.Interfaces;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Communication.Packets.Incoming.Structure
{
    internal class NavigatorHomeRoomEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopInt();
            RoomData roomData = WibboEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (RoomId != 0 && (roomData == null || roomData.OwnerName.ToLower() != Session.GetUser().Username.ToLower()))
            {
                return;
            }

            Session.GetUser().HomeRoom = RoomId;
            using (IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateHomeRoom(dbClient, Session.GetUser().Id, RoomId);
            }

            Session.SendPacket(new NavigatorHomeRoomComposer(RoomId, 0));
        }
    }
}
