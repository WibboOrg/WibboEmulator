using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
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
