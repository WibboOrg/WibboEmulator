using Butterfly.Communication.Packets.Outgoing.Navigator;

using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class UpdateNavigatorSettingsEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopInt();
            RoomData roomData = ButterflyEnvironment.GetGame().GetRoomManager().GenerateRoomData(RoomId);
            if (RoomId != 0 && (roomData == null || roomData.OwnerName.ToLower() != Session.GetHabbo().Username.ToLower()))
            {
                return;
            }

            Session.GetHabbo().HomeRoom = RoomId;
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                queryreactor.RunQuery("UPDATE users SET home_room = '" + RoomId + "' WHERE id = '" + Session.GetHabbo().Id + "';");
            }

            Session.SendPacket(new NavigatorSettingsComposer(RoomId));
        }
    }
}
