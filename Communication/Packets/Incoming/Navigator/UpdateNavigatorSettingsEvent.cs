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
            if (RoomId != 0 && (roomData == null || roomData.OwnerName.ToLower() != Session.GetHabbo().Username.ToLower()))
            {
                return;
            }

            Session.GetHabbo().HomeRoom = RoomId;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                UserDao.UpdateHomeRoom(dbClient, Session.GetHabbo().Id, RoomId);
            }

            Session.SendPacket(new NavigatorSettingsComposer(RoomId));
        }
    }
}
