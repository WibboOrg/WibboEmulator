using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UpdateNavigatorSettingsEvent : IPacketEvent
    {
        public double Delay => 1000;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopInt();
            RoomData roomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (RoomId != 0 && (roomData == null || roomData.OwnerName.ToLower() != Session.GetUser().Username.ToLower()))
            {
                return;
            }

            Session.GetUser().HomeRoom = RoomId;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateHomeRoom(dbClient, Session.GetUser().Id, RoomId);
            }

            Session.SendPacket(new NavigatorSettingsComposer(RoomId));
        }
    }
}
